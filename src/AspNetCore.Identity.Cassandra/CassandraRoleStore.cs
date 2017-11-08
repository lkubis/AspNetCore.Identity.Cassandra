using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Identity.Cassandra.Extensions;
using AspNetCore.Identity.Cassandra.Models;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Cassandra
{
    public class CassandraRoleStore<TRole, TSession> : 
        IRoleStore<TRole>
        where TRole : CassandraIdentityRole
        where TSession : class, ISession
    {
        #region | Fields

        private readonly IMapper _mapper;
        private bool _isDisposed;
        private readonly IOptionsSnapshot<CassandraOptions> _snapshot;
        private readonly ILogger _logger;

        #endregion

        #region | Properties

        public IdentityErrorDescriber ErrorDescriber { get; }
        public CassandraErrorDescriber CassandraErrorDescriber { get; }
        public TSession Session { get; }

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
            _snapshot = snapshot;
            _logger = loggerFactory.CreateLogger(typeof(CassandraRoleStore<,>).GetTypeInfo().Name);
        }

        #endregion

        public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return _mapper.TryInsertAsync(role, _snapshot.AsCqlQueryOptions(), CassandraErrorDescriber, _logger);
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return _mapper.TryUpdateAsync(role, _snapshot.AsCqlQueryOptions(), CassandraErrorDescriber, _logger);
        }

        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return _mapper.TryDeleteAsync(role, _snapshot.AsCqlQueryOptions(), CassandraErrorDescriber, _logger);
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

            return _mapper.SingleOrDefaultAsync<TRole>("WHERE Id = ?", roleId);
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();


            return _mapper.SingleOrDefaultAsync<TRole>("WHERE NormalizedName = ?", normalizedRoleName);
        }


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