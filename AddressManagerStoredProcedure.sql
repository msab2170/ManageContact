create procedure CreateCompany
	@Name  NVARCHAR (20),
	@Contact  NVARCHAR (12),
	@Address  NVARCHAR (50),

	@UserId int,
	@LoginId  nvarchar(MAX),
	@CompanyId int ,
	@CompanyName nvarchar(MAX),
	@ActIP nvarchar(MAX),
	@Act nvarchar(1)
AS
BEGIN
	insert into Company (Name, Contact, Address)
	values (@Name, @Contact, @Address)

	insert into ChangeHistory (UserId, LoginId, CompanyId, CompanyName, ActIP, Act)
	values (@UserId, @LoginId, @CompanyId, @CompanyName, @ActIP, @Act)
END
go

create procedure DorRCompany
	@id int,
	@DorR nvarchar(1),
	@UserId int,
	@LoginId  nvarchar(MAX),
	@ActIP nvarchar(MAX),
	@Act nvarchar(1)
as
begin
	declare @CompanyName NVARCHAR(MAX);

	select @CompanyName =Name from Company where Id = @id;

	update Company set IsDelete = @DorR where Id = @id;
	insert into ChangeHistory (UserId, LoginId, CompanyId, CompanyName, ActIP, Act)
	values (@UserId, @LoginId, @id, @CompanyName, @ActIP, @Act)
end
