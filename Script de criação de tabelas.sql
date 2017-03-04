BEGIN;
--CREATE DATABASE tccsharprecsys;

CREATE TABLE movie(
	id INT PRIMARY KEY,
	title text NOT NULL,
	year int NOT NULL,
	popularity int NOT NULL);

CREATE TABLE tag(
	id int PRIMARY KEY,
	tag text NOT NULL,
	popularity int NOT NULL);

CREATE TABLE tagrelevance(
	tag_id int NOT NULL REFERENCES tag(id),
	movie_id int NOT NULL REFERENCES movie(id),
	relevance numeric NOT NULL,
	PRIMARY KEY(tag_id, movie_id));

CREATE TABLE rating(
	user_id int NOT NULL,
	movie_id int NOT NULL REFERENCES movie(id),
	rating numeric NOT NULL,
	timestamp timestamp without time zone NOT NULL,
	PRIMARY KEY (user_id, movie_id));

CREATE TABLE somconfig(
	id int PRIMARY KEY,
	name text NOT NULL,
	rows int NOT NULL,
	columns int NOT NULL,
	metric text NOT NULL,
	neighborhood text NOT NULL,
	neighborhood_initial_width int NOT NULL,
	neighborhood_time_constant int NOT NULL,
	initial_learning_rate numeric NOT NULL,
	learning_rate_time_constant numeric NOT NULL,
	attr_count int NOT NULL,
	instance int NOT NULL,
	UNIQUE(rows, columns, metric, neighborhood, neighborhood_initial_width, neighborhood_time_constant, initial_learning_rate, learning_rate_time_constant, attr_count, instance)
);

CREATE TABLE somnodetag(
	x int NOT NULL,
	y int NOT NULL,
	somconfig_id int NOT NULL REFERENCES somconfig(id),
	tag_id int NOT NULL REFERENCES tag(id),
	value numeric NOT NULL,
	PRIMARY KEY(x, y, somconfig_id, tag_id));

CREATE TABLE sommovieclassification(
	somconfig_id int NOT NULL REFERENCES somconfig(id),
	movie_id int NOT NULL REFERENCES movie(id),
	x int NOT NULL,
	y int NOT NULL,
	PRIMARY KEY(somconfig_id, movie_id));

CREATE TABLE kmeansconfig(
	id int PRIMARY KEY,
	name text NOT NULL,
	cluster_count int NOT NULL,
	attr_count int NOT NULL,
	instance int NOT NULL,
	UNIQUE(cluster_count, attr_count, instance)
);

CREATE TABLE kmeansclustertag(
	id int NOT NULL,
	kmeansconfig_id int NOT NULL REFERENCES kmeansconfig(id),
	tag_id int NOT NULL REFERENCES tag(id),
	value numeric NOT NULL,
	PRIMARY KEY(id, kmeansconfig_id, tag_id));

CREATE TABLE kmeansmovieclassification(
	kmeansconfig_id int NOT NULL REFERENCES kmeansconfig(id),
	movie_id int NOT NULL REFERENCES movie(id),
	cluster_id int NOT NULL,
	PRIMARY KEY(kmeansconfig_id, movie_id));

COMMIT;