// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq;
using Dhgms.GripeWithRoslyn.Analyzer.Analyzers.EfCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dhgms.GripeWithRoslyn.Testing.EfCore
{
    /// <summary>
    /// Analyzer Proofs for <see cref="DbSet{TEntity}"/>.
    /// </summary>
    public static class DbSetUpdateProof
    {
        /// <summary>
        /// Proof of <see cref="DbSet{TEntity}.Update(TEntity)"/> method invocation to trigger <see cref="DoNotUseEntityFrameworkCoreDbSetUpdateAnalyzer"/>.
        /// </summary>
        /// <param name="dbContext">Identity Db Context instance.</param>
        /// <remarks>
        /// This code is just a proof for
        /// 1) making sure the code builds
        /// 2) making sure the analyzer triggers
        ///
        /// It is in no way meant to be regarded as usable code.
        /// </remarks>
        public static void CallsUpdate(IdentityDbContext dbContext)
        {
            var firstRecord = dbContext.Users.First();
            dbContext.Users.Update(firstRecord);
        }

        /// <summary>
        /// Proof of <see cref="DbSet{TEntity}.Update(TEntity)"/> method invocation to trigger <see cref="DoNotUseEntityFrameworkCoreDbSetUpdateRangeAnalyzer"/>.
        /// </summary>
        /// <param name="dbContext">Identity Db Context instance.</param>
        /// <example>
        /// <code>
        /// var dbContext = new IdentityDbContext();
        /// DbSetUpdateProof.CallsUpdateRange(dbContext);
        /// </code>
        /// </example>
        /// <remarks>
        /// This code is just a proof for
        /// 1) making sure the code builds
        /// 2) making sure the analyzer triggers
        ///
        /// It is in no way meant to be regarded as usable code.
        /// </remarks>
        public static void CallsUpdateRange(IdentityDbContext dbContext)
        {
            var allRecords = dbContext.Users.ToArray();
            dbContext.Users.UpdateRange(allRecords);
        }
    }
}
