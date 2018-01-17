using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Cassandra.Extensions;
using AspNetCore.Identity.Cassandra.Models;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Cassandra
{
    public class CassandraRoleStore<TRole, TSession> : 
        IQueryableRoleStore<TRole>
        where TRole : CassandraIdentityRole
        where TSession : class, ISession
    {
        #region | Fields

        private readonly IMapper _mapper;
        private readonly Table<TRole> _table;
        private bool _isDisposed;
        private readonly IOptionsSnapshot<CassandraOptions> _snapshot;
        private readonly ILogger _logger;

        #endregion

        #region | Properties

        public IdentityErrorDescriber ErrorDescriber { get; }
        public CassandraErrorDescriber CassandraErrorDescriber { get; }
        public TSession Session { get; }
        public IQueryable<TRole> Roles => _table;

        #endregion

        #region | Constructors

        public CassandraRoleStore(
            TSession session,
            IOptionsSnapshot<CassandraOptions> snapshot,
            ILoggerFactory loggerFactory,
            IdentityErrorDescriber errorDescriber = null,
            CassandraErrorDescriber cassandraErrorDescriber = null)
        {
            ErrorDescriber = errorDescriber;
            CassandraErrorDescriber = cassandraErrorDescriber;
            Session = session ?? throw new ArgumentNullException(nameof(session));

            _mapper = new Mapper(session);
            _table = new Table<TRole>(session);
            _snapshot = snapshot;
            _logger = loggerFactory.CreateLogger(typeof(CassandraRoleStore<,>).GetTypeInfo().Name);
        }

        #endregion

        #region | Public Methods

        public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return _mapper.TryInsertAsync(role, _snapshot.AsCqlQueryOptions(), CassandraErrorDescriber, _logger);
        }

        public async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var options = _snapshot.Value;
            var originalRole = await FindByIdAsync(role.Id.ToString(), cancellationToken);
            var affectedUsers = (await _mapper.FetchAsync<Guid>(
                $"SELECT id FROM {options.KeyspaceName}.{CassandraSessionHelper.UsersTableName} WHERE roles CONTAINS ?",
                originalRole.NormalizedName)).ToList();

            // Role cannot be changed directly in a list. We have to remove it first, and then add it again
            return await _mapper.TryExecuteBatchAsync(CassandraErrorDescriber, _logger, options.Query,
                batch =>
                {
                    // Remove original role from existing users
                    if (!affectedUsers.Any())
                        return;

                    batch.Execute(
                        $"UPDATE {options.KeyspaceName}.{CassandraSessionHelper.UsersTableName} SET roles = roles - ['{originalRole.NormalizedName}'] WHERE Id IN ?",
                        affectedUsers);
                },
                batch =>
                {
                    // Add updated role to existing users
                    if (!affectedUsers.Any())
                        return;

                    batch.Execute(
                        $"UPDATE {options.KeyspaceName}.{CassandraSessionHelper.UsersTableName} SET roles = roles + ['{role.NormalizedName}'] WHERE Id IN ?",
                        affectedUsers);
                },
                batch => batch.Update(role));
        }

        public async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var options = _snapshot.Value;
            var affectedUsers = (await _mapper.FetchAsync<Guid>(
                $"SELECT id FROM {options.KeyspaceName}.{CassandraSessionHelper.UsersTableName} WHERE roles CONTAINS ?",
                role.NormalizedName)).ToList();

            return await _mapper.TryExecuteBatchAsync(CassandraErrorDescriber, _logger, options.Query,
                batch =>
                {
                    if (!affectedUsers.Any())
                        return;

                    batch.Execute(
                        $"UPDATE {options.KeyspaceName}.{CassandraSessionHelper.UsersTableName} SET roles = roles - ['{role.NormalizedName}'] WHERE Id IN ?",
                        affectedUsers);
                },
                batch => batch.Delete(role));
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.Name = roleName;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return _mapper.SingleOrDefaultAsync<TRole>("WHERE Id = ?", Guid.Parse(roleId));
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return _mapper.SingleOrDefaultAsync<TRole>("FROM roles_by_name WHERE NormalizedName = ?", normalizedRoleName);
        }

        #endregion

        #region | IDisposable

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public void Dispose()
        {
            _isDisposed = true;
        }

        #endregion
    }
}