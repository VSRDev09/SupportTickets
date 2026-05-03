CREATE TYPE status_chamado AS ENUM (
    'ABERTO',
    'EXECUTANDO',
    'FINALIZADO',
    'CANCELADO'
);

CREATE TYPE perfil_usuario AS ENUM (
    'ADMIN',
    'ATENDENTE',
    'USUARIO'
);

CREATE TABLE setores (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(100) NOT NULL UNIQUE,
    descricao TEXT
);

CREATE TABLE prioridades (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(50) NOT NULL UNIQUE,
    tempo_estimado_horas INT NOT NULL,
    CONSTRAINT ck_prioridades_tempo_estimado_horas CHECK (tempo_estimado_horas > 0)
);

CREATE TABLE usuarios (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(150) NOT NULL,
    email VARCHAR(150) NOT NULL UNIQUE,
    senha_hash TEXT NOT NULL,
    perfil perfil_usuario NOT NULL,
    setor_id INT NOT NULL,
    criado_em TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    ativo BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT fk_usuarios_setores
        FOREIGN KEY (setor_id)
        REFERENCES setores(id)
        ON DELETE RESTRICT
);

CREATE TABLE chamados (
    id SERIAL PRIMARY KEY,
    titulo VARCHAR(200) NOT NULL,
    descricao TEXT NOT NULL,
    usuario_id INT NOT NULL,
    setor_id INT NOT NULL,
    prioridade_id INT NOT NULL,
    status status_chamado NOT NULL DEFAULT 'ABERTO',
    criado_em TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    finalizado_em TIMESTAMPTZ NULL,
    finalizado_por INT NULL,
    cancelado_em TIMESTAMPTZ NULL,
    cancelado_por INT NULL,
    CONSTRAINT fk_chamados_usuarios
        FOREIGN KEY (usuario_id)
        REFERENCES usuarios(id)
        ON DELETE RESTRICT,
    CONSTRAINT fk_chamados_setores
        FOREIGN KEY (setor_id)
        REFERENCES setores(id)
        ON DELETE RESTRICT,
    CONSTRAINT fk_chamados_prioridades
        FOREIGN KEY (prioridade_id)
        REFERENCES prioridades(id)
        ON DELETE RESTRICT,
    CONSTRAINT fk_chamados_finalizado_por
        FOREIGN KEY (finalizado_por)
        REFERENCES usuarios(id)
        ON DELETE RESTRICT,
    CONSTRAINT fk_chamados_cancelado_por
        FOREIGN KEY (cancelado_por)
        REFERENCES usuarios(id)
        ON DELETE RESTRICT,
    CONSTRAINT ck_chamados_finalizacao CHECK (
        status <> 'FINALIZADO'
        OR (finalizado_em IS NOT NULL AND finalizado_por IS NOT NULL)
    ),
    CONSTRAINT ck_chamados_cancelamento CHECK (
        status <> 'CANCELADO'
        OR (cancelado_em IS NOT NULL AND cancelado_por IS NOT NULL)
    )
);

CREATE TABLE chamado_atendimentos (
    id SERIAL PRIMARY KEY,
    chamado_id INT NOT NULL UNIQUE,
    atendente_id INT NOT NULL,
    iniciado_em TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    finalizado_em TIMESTAMPTZ NULL,
    solucao TEXT NULL,
    CONSTRAINT fk_chamado_atendimentos_chamados
        FOREIGN KEY (chamado_id)
        REFERENCES chamados(id)
        ON DELETE CASCADE,
    CONSTRAINT fk_chamado_atendimentos_usuarios
        FOREIGN KEY (atendente_id)
        REFERENCES usuarios(id)
        ON DELETE RESTRICT,
    CONSTRAINT ck_chamado_atendimentos_datas CHECK (
        finalizado_em IS NULL
        OR finalizado_em > iniciado_em
    )
);

CREATE TABLE chamado_status_historico (
    id SERIAL PRIMARY KEY,
    chamado_id INT NOT NULL,
    status status_chamado NOT NULL,
    alterado_por INT NOT NULL,
    alterado_em TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_chamado_status_historico_chamados
        FOREIGN KEY (chamado_id)
        REFERENCES chamados(id)
        ON DELETE CASCADE,
    CONSTRAINT fk_chamado_status_historico_usuarios
        FOREIGN KEY (alterado_por)
        REFERENCES usuarios(id)
        ON DELETE RESTRICT
);

CREATE INDEX ix_chamados_status ON chamados(status);
CREATE INDEX ix_chamados_setor_id ON chamados(setor_id);
CREATE INDEX ix_chamados_prioridade_id ON chamados(prioridade_id);
CREATE INDEX ix_chamados_criado_em ON chamados(criado_em DESC);
CREATE INDEX ix_chamado_status_historico_chamado_id ON chamado_status_historico(chamado_id, alterado_em DESC);

-- Observacao:
-- O historico registra apenas o novo status.
-- O campo status_anterior foi mantido como evolucao futura para nao aumentar a complexidade agora.
