CREATE DATABASE decidio;

CREATE TABLE emails (
	email varchar(250) not null primary key,
	lastsent timestamp not null
);