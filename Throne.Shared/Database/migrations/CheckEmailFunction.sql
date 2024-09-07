CREATE OR REPLACE FUNCTION check_email_exists(
    p_email VARCHAR
)
RETURNS BOOLEAN
LANGUAGE plpgsql
AS $$
DECLARE
    email_exists BOOLEAN;
BEGIN
    SELECT EXISTS (SELECT 1 FROM accounts WHERE email = p_email) INTO email_exists;
    RETURN email_exists;
END;
$$;
