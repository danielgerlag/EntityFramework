// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Storage.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class SqliteDateTimeOffsetTypeMapping : DateTimeOffsetTypeMapping
    {
        private const string DateTimeOffsetFormatConst = @"{0:yyyy\-MM\-dd HH\:mm\:ss.FFFFFFFzzz}";

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqliteDateTimeOffsetTypeMapping" /> class.
        /// </summary>
        /// <param name="storeType"> The name of the database type. </param>
        /// <param name="dbType"> The <see cref="System.Data.DbType" /> to be used. </param>
        public SqliteDateTimeOffsetTypeMapping(
            [NotNull] string storeType,
            [CanBeNull] DbType? dbType = null)
            : base(storeType, dbType)
        {
        }

        /// <summary>
        ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public override RelationalTypeMapping CreateCopy(string storeType, int? size)
            => new SqliteDateTimeOffsetTypeMapping(storeType, DbType);

        /// <summary>
        ///     Gets the string format to be used to generate SQL literals of this type.
        /// </summary>
        protected override string SqlLiteralFormatString => "'" + DateTimeOffsetFormatConst + "'";
    }
}
