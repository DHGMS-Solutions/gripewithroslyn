// Copyright (c) 2019 DHGMS Solutions and Contributors. All rights reserved.
// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gripe.Testing.EfCore
{
    /// <summary>
    /// Test query for running an EFCore query without using a projection.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class GetListWithoutSelection
    {
        private readonly IDbContextFactory<IdentityDbContext> _dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetListWithoutSelection"/> class.
        /// </summary>
        /// <param name="dbContextFactory">Factory for creating DbContext instances.</param>
        public GetListWithoutSelection(IDbContextFactory<IdentityDbContext> dbContextFactory)
        {
            ArgumentNullException.ThrowIfNull(dbContextFactory);
            _dbContextFactory = dbContextFactory;
        }

        /// <summary>
        /// Gets a list of users.
        /// </summary>
        /// <returns>List of users.</returns>
        public async Task<List<IdentityUser>> GetResult()
        {
            using (var dbContext = await _dbContextFactory.CreateDbContextAsync().ConfigureAwait(false))
            {
                return await dbContext.Users.ToListAsync().ConfigureAwait(false);
            }
        }
    }
}
