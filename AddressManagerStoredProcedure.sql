drop procedure if exists CreateCompany 
go
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

drop procedure if exists DorRCompany 
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

	declare @WorkerCountInCompany int;
	select @WorkerCountInCompany=count(CompanyId)  from Worker group by CompanyId having CompanyId = @id
	if @WorkerCountInCompany > 0 and @DorR = 'Y'
	begin
		update Worker set IsDelete = 'Y' where CompanyId = @id
	end
end
go

drop procedure if exists CreateWorker 
go
create procedure CreateWorker
	@CompanyId int,
	@Name nvarchar(MAX),
	@Email nvarchar(MAX),
	@Phone nvarchar(MAX),

	@UserId int,
	@LoginId nvarchar(MAX),
	@ActIP nvarchar(MAX)
AS
begin
	insert into Worker (CompanyId, Name, Email, Phone)
	values (@CompanyId, @Name, @Email, @Phone);

	declare @WorkerId int;
	select @WorkerId =Max(Id) from Worker;

	declare @CompanyName nvarchar(MAX);
	select @CompanyName = Name from Company where Id = @CompanyId;

	insert into ChangeHistory (UserId, LoginId, CompanyId, CompanyName, WorkerId, WorkerName, ActIP, Act)
	values (@UserId, @LoginId, @CompanyId, @CompanyName, @WorkerId, @Name, @ActIP, 'C')
end
go

drop procedure if exists DorRWorker
go
create procedure DorRWorker
	@id int,
	@DorR nvarchar(1),
	@UserId int,
	@LoginId  nvarchar(MAX),
	@ActIP nvarchar(MAX),
	@Act nvarchar(1)
as
begin
	declare @WorkerName NVARCHAR(MAX);
	declare @CompanyId int;
	declare @CompanyName nvarchar(MAX);
	declare @CompanyIdCheck int;
	select @WorkerName =Name, @CompanyId = CompanyId from Worker where Id = @id;	
	--select @CompanyIdCheck = count(Id)  from Company where Id = @CompanyId and IsDelete = 'N';

	update Worker set IsDelete = @DorR where Id = @id;
	select @CompanyName = Name from Company where Id = @CompanyId and IsDelete = 'N';
	insert into ChangeHistory (UserId, LoginId, CompanyId, CompanyName, WorkerId, WorkerName, ActIP, Act)
	values (@UserId, @LoginId, @CompanyId, @CompanyName, @id, @WorkerName, @ActIP, @Act)
	select '0';
	RETURN;
end
