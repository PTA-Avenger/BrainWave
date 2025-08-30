using BrainWave.API.Entities;
using Microsoft.Data.Sqlite;
using Npgsql;
using System.Data.Common;

public class TasksRepository
{
    private readonly string _connectionString;
    private readonly string _provider;

    public TasksRepository(IConfiguration config)
    {
        _provider = config.GetValue<string>("UseProvider")?.ToLower() ?? "sqlite";
        _connectionString = _provider == "postgres"
            ? config.GetConnectionString("Postgres")
            : config.GetConnectionString("Sqlite");
    }

    private DbConnection CreateConnection()
    {
        if (_provider == "postgres")
            return new NpgsqlConnection(_connectionString);
        else
            return new SqliteConnection(_connectionString);
    }

    public async Task<List<Tasks>> GetTasksByUserIdAsync(int userId)
    {
        var tasks = new List<Tasks>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? @"SELECT ""taskid"", ""userid"", ""title"", ""description"", ""due_date"", ""task_status"", ""priority_level"" 
                FROM tasks WHERE ""userid"" = @userId"
            : @"SELECT TaskID, UserID, Title, Description, Due_Date, Task_Status, Priority_Level 
                FROM Tasks WHERE UserID = @userId";
        cmd.Parameters.Add(CreateParameter(cmd, "@userId", userId));

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(new Tasks
            {
                TaskID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                Title = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Due_Date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                Task_Status = reader.IsDBNull(5) ? null : reader.GetString(5),
                Priority_Level = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }
        return tasks;
    }

    public async Task<Tasks?> GetTaskByIdAsync(int id)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? @"SELECT ""taskid"", ""userid"", ""title"", ""description"", ""due_date"", ""task_status"", ""priority_level"" 
                FROM tasks WHERE ""taskid"" = @id"
            : @"SELECT TaskID, UserID, Title, Description, Due_Date, Task_Status, Priority_Level 
                FROM Tasks WHERE TaskID = @id";
        cmd.Parameters.Add(CreateParameter(cmd, "@id", id));

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Tasks
            {
                TaskID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                Title = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Due_Date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                Task_Status = reader.IsDBNull(5) ? null : reader.GetString(5),
                Priority_Level = reader.IsDBNull(6) ? null : reader.GetString(6)
            };
        }
        return null;
    }

    public async Task AddTaskAsync(Tasks task)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? @"INSERT INTO tasks (""userid"", ""title"", ""description"", ""due_date"", ""task_status"", ""priority_level"")
                VALUES(@UserID, @Title, @Description, @Due_Date, @Task_Status, @Priority_Level)
                RETURNING ""taskid"";"
            : @"INSERT INTO Tasks (UserID, Title, Description, Due_Date, Task_Status, Priority_Level)
                VALUES (@UserID, @Title, @Description, @Due_Date, @Task_Status, @Priority_Level);
                SELECT last_insert_rowid();";

        cmd.Parameters.Add(CreateParameter(cmd, "@UserID", task.UserID));
        cmd.Parameters.Add(CreateParameter(cmd, "@Title", task.Title));
        cmd.Parameters.Add(CreateParameter(cmd, "@Description", (object?)task.Description ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Due_Date", (object?)task.Due_Date ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Task_Status", (object?)task.Task_Status ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Priority_Level", (object?)task.Priority_Level ?? DBNull.Value));

        if (_provider == "postgres")
        {
            var result = await cmd.ExecuteScalarAsync();
            task.TaskID = Convert.ToInt32(result);
        }
        else
        {
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = "SELECT last_insert_rowid();";
            var result = await cmd.ExecuteScalarAsync();
            task.TaskID = Convert.ToInt32(result);
        }
    }

    public async Task UpdateTaskAsync(Tasks task)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? @"UPDATE tasks
                   SET ""title"" = @Title,
                       ""description"" = @Description,
                       ""due_date"" = @Due_Date,
                       ""task_status"" = @Task_Status,
                       ""priority_level"" = @Priority_Level
                 WHERE ""taskid"" = @TaskID"
            : @"UPDATE Tasks
                   SET Title = @Title,
                       Description = @Description,
                       Due_Date = @Due_Date,
                       Task_Status = @Task_Status,
                       Priority_Level = @Priority_Level
                 WHERE TaskID = @TaskID";

        cmd.Parameters.Add(CreateParameter(cmd, "@Title", task.Title));
        cmd.Parameters.Add(CreateParameter(cmd, "@Description", (object?)task.Description ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Due_Date", (object?)task.Due_Date ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Task_Status", (object?)task.Task_Status ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@Priority_Level", (object?)task.Priority_Level ?? DBNull.Value));
        cmd.Parameters.Add(CreateParameter(cmd, "@TaskID", task.TaskID));

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteTaskAsync(Tasks task)
    {
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? @"DELETE FROM tasks WHERE ""taskid"" = @TaskID"
            : @"DELETE FROM Tasks WHERE TaskID = @TaskID";
        cmd.Parameters.Add(CreateParameter(cmd, "@TaskID", task.TaskID));
        await cmd.ExecuteNonQueryAsync();
    }

    private static DbParameter CreateParameter(DbCommand cmd, string name, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.Value = value;
        return param;
    }

    public async Task<List<Tasks>> GetAllTasks()
    {
        var tasks = new List<Tasks>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = _provider == "postgres"
            ? @"SELECT ""taskid"", ""userid"", ""title"", ""description"", ""due_date"", ""task_status"", ""priority_level"" 
                FROM tasks"
            : @"SELECT TaskID, UserID, Title, Description, Due_Date, Task_Status, Priority_Level 
                FROM Tasks";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(new Tasks
            {
                TaskID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                Title = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Due_Date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                Task_Status = reader.IsDBNull(5) ? null : reader.GetString(5),
                Priority_Level = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }
        return tasks;
    }

    public async Task<List<BrainWave.Api.DTOs.TaskDtos>> GetFilteredTasksAsync(int userId, BrainWave.Api.DTOs.TaskFilterDto? filter = null)
    {
        var tasks = new List<BrainWave.Api.DTOs.TaskDtos>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();

        var whereClause = @"WHERE ""userid"" = @userId";
        var parameters = new List<DbParameter>();

        parameters.Add(CreateParameter(cmd, "@userId", userId));

        if (!string.IsNullOrEmpty(filter?.Status))
        {
            whereClause += @" AND ""task_status"" = @status";
            parameters.Add(CreateParameter(cmd, "@status", filter.Status));
        }

        if (!string.IsNullOrEmpty(filter?.Priority))
        {
            whereClause += @" AND ""priority_level"" = @priority";
            parameters.Add(CreateParameter(cmd, "@priority", filter.Priority));
        }

        var orderBy = $@"ORDER BY ""{filter?.SortBy?.ToLower() ?? "taskid"}"" {(filter?.SortDescending == true ? "DESC" : "ASC")}";

        cmd.CommandText = _provider == "postgres"
            ? $@"SELECT ""taskid"", ""userid"", ""title"", ""description"", ""due_date"", ""task_status"", ""priority_level"" 
                 FROM tasks {whereClause} {orderBy}"
            : $@"SELECT TaskID, UserID, Title, Description, Due_Date, Task_Status, Priority_Level 
                 FROM Tasks {whereClause.Replace("\"", "")} {orderBy.Replace("\"", "")}";

        foreach (var param in parameters)
            cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(new BrainWave.Api.DTOs.TaskDtos
            {
                TaskID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                Title = reader.GetString(2),
                Description = reader.IsDBNull(3) ? null : reader.GetString(3),
                Due_Date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                Task_Status = reader.IsDBNull(5) ? "Pending" : reader.GetString(5),
                Priority_Level = reader.IsDBNull(6) ? "Medium" : reader.GetString(6)
            });
        }
        return tasks;
    }

    public async Task<List<BrainWave.Api.DTOs.AdminTaskDto>> GetAllFilteredTasksAsync(BrainWave.Api.DTOs.TaskFilterDto? filter = null)
    {
        var tasks = new List<BrainWave.Api.DTOs.AdminTaskDto>();
        using var conn = CreateConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();

        var whereClause = "";
        var parameters = new List<DbParameter>();

        if (!string.IsNullOrEmpty(filter?.Status))
        {
            whereClause = @"WHERE t.""task_status"" = @status";
            parameters.Add(CreateParameter(cmd, "@status", filter.Status));
        }

        if (!string.IsNullOrEmpty(filter?.Priority))
        {
            whereClause += (whereClause.Length > 0 ? " AND " : "WHERE ") + @"t.""priority_level"" = @priority";
            parameters.Add(CreateParameter(cmd, "@priority", filter.Priority));
        }

        var orderBy = $@"ORDER BY t.""{filter?.SortBy?.ToLower() ?? "taskid"}"" {(filter?.SortDescending == true ? "DESC" : "ASC")}";

        cmd.CommandText = _provider == "postgres"
            ? $@"SELECT t.""taskid"", t.""userid"", COALESCE(u.""f_name"" || ' ' || u.""l_name"", 'Unknown') as username, 
                         t.""title"", t.""description"", t.""due_date"", t.""task_status"", t.""priority_level"" 
                  FROM tasks t 
                  LEFT JOIN users u ON t.""userid"" = u.""userid"" {whereClause} {orderBy}"
            : $@"SELECT t.TaskID, t.UserID, COALESCE(u.F_Name || ' ' || u.L_Name, 'Unknown') as UserName, 
                         t.Title, t.Description, t.Due_Date, t.Task_Status, t.Priority_Level 
                  FROM Tasks t 
                  LEFT JOIN Users u ON t.UserID = u.UserID {whereClause.Replace("\"", "")} {orderBy.Replace("\"", "")}";

        foreach (var param in parameters)
            cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(new BrainWave.Api.DTOs.AdminTaskDto
            {
                TaskID = reader.GetInt32(0),
                UserID = reader.GetInt32(1),
                UserName = reader.IsDBNull(2) ? "Unknown" : reader.GetString(2),
                Title = reader.GetString(3),
                Description = reader.IsDBNull(4) ? null : reader.GetString(4),
                Due_Date = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                Task_Status = reader.IsDBNull(6) ? "Pending" : reader.GetString(6),
                Priority_Level = reader.IsDBNull(7) ? "Medium" : reader.GetString(7)
            });
        }
        return tasks;
    }
}
