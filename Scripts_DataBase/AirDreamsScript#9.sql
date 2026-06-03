UPDATE AirlineEmployee
SET isAdmin = 1,
    isOperator = 0
WHERE emailInternalUser = 'admin3@air.com';

CREATE TRIGGER TR_PreventMainAdminUpdate
ON AirlineEmployee
INSTEAD OF UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM deleted
        WHERE emailInternalUser = 'admin3@air.com'
    )
    BEGIN
        RAISERROR(
            'No se puede modificar el administrador principal.',
            16,
            1
        );
        RETURN;
    END

    UPDATE ae
    SET
        ae.nameEmployee = i.nameEmployee,
        ae.lastnames = i.lastnames,
        ae.isAdmin = i.isAdmin,
        ae.isOperator = i.isOperator
    FROM AirlineEmployee ae
    INNER JOIN inserted i
        ON ae.employeeID = i.employeeID;
END;
GO
