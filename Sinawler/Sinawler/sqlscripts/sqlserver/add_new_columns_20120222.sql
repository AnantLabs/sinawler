/*
2012-3-17
�ɰ汾�����ݿ⣬��ֱ���������½ű������ֶ�
����ֱ���������ű������������д˽ű�
*/
ALTER TABLE users ADD idstr VARCHAR(50);
ALTER TABLE users ADD weihao VARCHAR(50);
ALTER TABLE users ADD avatar_large VARCHAR(100);
ALTER TABLE users ADD bi_followers_count INT;
ALTER TABLE users ADD verified_type INT;
ALTER TABLE users ADD verified_reason VARCHAR(100);
ALTER TABLE users ADD allow_all_comment bit;
ALTER TABLE users ADD online_status INT;
ALTER TABLE users ADD lang VARCHAR(50);

ALTER TABLE tags ADD [weight] int;