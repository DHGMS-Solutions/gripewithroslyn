using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dhgms.GripeWithRoslyn.IntegrationTests
{
    using FluentData;

    public sealed class FluentDataTests
    {
        public sealed class QueryManyMethodTests
        {
            private const string Sql = "SELECT 1 ID";

            public void QueryManyWithNullMapperProducesWarning()
            {
                using (var dbContext = new DbContext())
                {
                    dbContext.Select<SampleDto>(Sql).QueryMany(null);
                }
            }
            public void QueryManyWithMapperNoWarning()
            {
                using (var dbContext = new DbContext())
                {
                    dbContext.Select<SampleDto>(Sql).QueryMany(SampleDtoMapper);
                }
            }

            private static void SampleDtoMapper(SampleDto sampleDto, IDataReader dataReader)
            {
                sampleDto.Id = dataReader.GetInt32("id");
            }
        }

        public sealed class AutoMapMethodTest
        {
            public void InsertGivesWarningForUsingMethod()
            {
                using (var dbContext = new DbContext())
                {
                    dbContext.Insert("table", new SampleDto()).AutoMap();
                }
            }

            public void UpdateGivesWarningForUsingMethod()
            {
                using (var dbContext = new DbContext())
                {
                    dbContext.Update("table", new SampleDto()).AutoMap();
                }
            }

            public void StoredProcedureGivesWarningForUsingMethod()
            {
                using (var dbContext = new DbContext())
                {
                    dbContext.StoredProcedure("storedproc", new SampleDto()).AutoMap();
                }
            }
        }
    }
}
