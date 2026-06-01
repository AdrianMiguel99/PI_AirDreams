UPDATE AirlineEmployee
SET isAdmin = 1,
    isOperator = 0
WHERE emailInternalUser = 'admin3@air.com';