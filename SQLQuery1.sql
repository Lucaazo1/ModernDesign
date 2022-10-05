Create Table Settings{
    deviceId varchar(450) not null primary key,
    connectionString varchar(max) null,
    deviceName nvarchar(50) null,
    deviceType nvarchar(50) null,
    Location location(50) null
    }
