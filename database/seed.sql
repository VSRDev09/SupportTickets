INSERT INTO setores (nome, descricao) VALUES
('TI', 'Setor responsavel por infraestrutura, sistemas e suporte tecnico.'),
('RH', 'Setor responsavel por rotinas de recursos humanos.'),
('Financeiro', 'Setor responsavel por pagamentos, faturamento e controles financeiros.');

INSERT INTO prioridades (nome, tempo_estimado_horas) VALUES
('Baixa', 8),
('Media', 4),
('Alta', 1);

INSERT INTO usuarios (nome, email, senha_hash, perfil, setor_id, ativo) VALUES
(
    'Administrador do Sistema',
    'admin@supporttickets.local',
    'pbkdf2-sha256.100000.3ni+B7gg/6e+rTNcGgKcrg==.SzzgvEIxJCQICIkGVQtqdhPNtEl2YZsU9UIcKGe9+Kg=',
    'ADMIN',
    1,
    TRUE
),
(
    'Atendente Padrao',
    'atendente@supporttickets.local',
    'pbkdf2-sha256.100000.VB7ckfWUNAdQxdHkz4d8SA==.ts5Tqdm5HklXjUaY4fLpyO8AZw0w8df880XS6mEir2s=',
    'ATENDENTE',
    1,
    TRUE
),
(
    'Usuario Padrao',
    'usuario@supporttickets.local',
    'pbkdf2-sha256.100000.dU/NDktONw9ZoMe3XbWGGg==.6tyUuvwdj42ytNL4D1laFY5PpMxlHskw2uJpDqQUNzA=',
    'USUARIO',
    2,
    TRUE
);
