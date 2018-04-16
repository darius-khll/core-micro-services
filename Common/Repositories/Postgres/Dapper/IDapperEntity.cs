using System;

namespace Common.Repositories.Postgres.Dapper
{
    public interface IDapperEntity
    {
        Guid Id { get; set; }
    }
}
