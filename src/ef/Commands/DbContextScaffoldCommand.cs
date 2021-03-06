﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Tools.Properties;

namespace Microsoft.EntityFrameworkCore.Tools.Commands
{
    partial class DbContextScaffoldCommand
    {
        protected override void Validate()
        {
            base.Validate();

            if (string.IsNullOrEmpty(_connection.Value))
            {
                throw new CommandException(Resources.MissingArgument(_connection.Name));
            }
            if (string.IsNullOrEmpty(_provider.Value))
            {
                throw new CommandException(Resources.MissingArgument(_provider.Name));
            }
        }

        protected override int Execute()
        {
            var result = CreateExecutor().ScaffoldContext(
                _provider.Value,
                _connection.Value,
                _outputDir.Value(),
                _context.Value(),
                _schemas.Values,
                _tables.Values,
                _dataAnnotations.HasValue(),
                _force.HasValue());
            if (_json.HasValue())
            {
                ReportJsonResults(result);
            }

            return base.Execute();
        }

        private void ReportJsonResults(IDictionary result)
        {
            Reporter.WriteData("{");
            Reporter.WriteData("  \"contextFile\": \"" + Json.Escape(result["ContextFile"] as string) + "\",");
            Reporter.WriteData("  \"entityTypeFiles\": [");

            var files = (IReadOnlyList<string>)result["EntityTypeFiles"];
            for (var i = 0; i < files.Count; i++)
            {
                var line = "    \"" + Json.Escape(files[i]) + "\"";
                if (i != files.Count - 1)
                {
                    line += ",";
                }

                Reporter.WriteData(line);
            }

            Reporter.WriteData("  ]");
            Reporter.WriteData("}");

        }
    }
}
