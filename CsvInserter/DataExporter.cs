﻿using System;
using System.Collections.Generic;
using System.Data;
using FileHelpers;

namespace CsvInserter
{
    public class DataExporter
    {
        private readonly ConsoleLogger _logger;
        private readonly string _serverName;
        private readonly string _destinationDirectory;
        private readonly QueryExecutor _queryExecutor;

        public DataExporter(ConsoleLogger logger, string serverName, string destinationDirectory,
                            QueryExecutor queryExecutor)
        {
            _logger = logger;
            _queryExecutor = queryExecutor;
            _destinationDirectory = destinationDirectory;
            _serverName = serverName;
        }

        public void TearDownFilterTables(List<SqlTableSelect> filtertableSelects, string databaseName)
        {
            bool fail = false;
            string failedSteps = string.Empty;
            _logger.Log("Tearing Down:");
            foreach (var table in filtertableSelects)
            {
                _logger.Log("     " + table.TableName);
                try
                {
                    _queryExecutor.ExecuteNonQueryStatement("drop table " + table.TableName,
                                                            "server=" + _serverName +
                                                            ";Integrated Security=SSPI;Initial Catalog=" +
                                                            databaseName);
                }
                catch (Exception e)
                {
                    _logger.Log("         Failed " + table.TableName);
                    fail = true;
                    failedSteps += " " + table.TableName;
                }
            }
            if (fail)
                throw new TearDownException("one or more teardown steps failed:" + failedSteps);
        }

        public void SetupFilterTables(List<SqlTableSelect> filtertableSelects, string databaseName)
        {
            _logger.Log("Setting Up:");
            foreach (var table in filtertableSelects)
            {
                _logger.Log("     " + table.TableName);
                _queryExecutor.ExecuteNonQueryStatement(table.Select,
                                                        "server=" + _serverName +
                                                        ";Integrated Security=SSPI;Initial Catalog=" +
                                                        databaseName);
            }
        }

        public void GenerateCsvs(List<SqlTableSelect> selects, string databaseName)
        {
            _logger.Log("Generating Csv:");

            foreach (var table in selects)
            {
                _logger.Log("     " + table.TableName);
                var select = table.Select != string.Empty ? table.Select : "select * from " + table.TableName;
                DataTable results =
                    _queryExecutor.ExecuteSelectStatement(select,
                                                          "server=" + _serverName +
                                                          ";Integrated Security=SSPI;Initial Catalog=" + databaseName);
                foreach (var column in table.ExcludedColumns)
                {
                    results.Columns.Remove(column);
                }

                var csvOptions = new CsvOptions("blah", ',', results.Columns.Count);

                CsvEngine.DataTableToCsv(results, _destinationDirectory + table.TableName.ToLower() + ".csv", csvOptions);
            }
        }
    }
}