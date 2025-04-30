CREATE TABLE Bingo (
                       id SERIAL PRIMARY KEY,
                       bingo VARCHAR(255) NOT NULL,
                       description TEXT
);

CREATE TABLE Question (
                          id SERIAL PRIMARY KEY,
                          pack_title VARCHAR(255) NOT NULL,
                          date TIMESTAMP NOT NULL,
                          number INT NOT NULL,
                          text TEXT NOT NULL,
                          additional_material_text TEXT,
                          additional_material_picture_url VARCHAR(255),
                          answer TEXT NOT NULL,
                          accepted_answer TEXT,
                          not_accepted_answer TEXT,
                          comment TEXT,
                          note TEXT,
                          author VARCHAR(255),
                          editor VARCHAR(255),
                          bingo_id INT NOT NULL,
                          FOREIGN KEY (bingo_id) REFERENCES Bingo(id)
);
