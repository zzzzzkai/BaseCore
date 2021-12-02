using DataModel;
using Repository.IRepository;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class TokenService : BaseService<Tokens>, ITokenService
    {
        private readonly ITokensRepository _tokensRepository;

        public TokenService(ITokensRepository tokensRepository)
        {
            _tokensRepository = tokensRepository;
            base._baseRepository = _tokensRepository;
        }

        /// <summary>
        /// 保存token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool SaveToken(Tokens token)
        {
            var r=_tokensRepository.Insert(token);
            if (r == null) {
                return  false;
            }
            return true;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Tokens GetToken(string refreshToken, string userName)
        {
            return _tokensRepository.FindByClause(x => x.refreshToken == refreshToken && x.userName == userName);
        }

        /// <summary>
        /// 删除token
        /// </summary>
        /// <param name="refreshToken">刷新的token</param>
        /// <param name="userName">用户</param>
        /// <returns></returns>
        public bool DelToken(string refreshToken, string userName)
        {
             
            return _tokensRepository.ExecuteSqlExecuteSqlCommand("delete tokens where RefreshToken=@RefreshToken and UserName=@UserName ", new { RefreshToken = refreshToken, UserName = userName }) > 0 ? true : false;
        }

        /// <summary>
        /// 更新当前token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool UpdataToken(Tokens token)
        {
            return true;
            //return _tokensRepository.Update(token);
        }
    }
}
