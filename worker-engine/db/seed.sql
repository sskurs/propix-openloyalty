CREATE TABLE IF NOT EXISTS earning_rules (
  id varchar(255) PRIMARY KEY,
  name varchar(255)
);
CREATE TABLE IF NOT EXISTS transaction_events (
  id serial PRIMARY KEY,
  transaction_id varchar(255),
  user_id varchar(255),
  amount numeric,
  raw_payload text,
  created_at timestamp default now()
);
CREATE TABLE IF NOT EXISTS outbox_messages (
  id varchar(255) PRIMARY KEY,
  topic varchar(255),
  payload text,
  status varchar(50),
  created_at timestamp default now(),
  sent_at timestamp
);

