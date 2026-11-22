
CREATE DATABASE IF NOT EXISTS studyup;

USE studyup;

CREATE TABLE disciplina (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL
);

CREATE TABLE catalogo_atividade (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL
);

CREATE TABLE atividade (
    id INT PRIMARY KEY AUTO_INCREMENT,
    catalogo_id INT NOT NULL,
    disciplina_id INT NOT NULL,
    data_cadastro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    data_planejada DATE NOT NULL,
    status VARCHAR(30) NOT NULL DEFAULT 'Pendente',

    FOREIGN KEY (catalogo_id)
        REFERENCES catalogo_atividade(id),

    FOREIGN KEY (disciplina_id)
        REFERENCES disciplina(id)
);
