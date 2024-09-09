using BackEnd.core.Entities;

namespace BackEnd.src.infrastructure.DataAccess.IRepository
{
    public interface IRoleRepository
    {
        Task<List<Roles>> _GetListOfRoles();
        Task _AddRole(Roles role);
        Task<List<Roles>> _GetRoleBy_ID(string id);
        Task<bool> _EditRoleBy_ID(string ID, Roles role);
        Task<bool> _DeleteRoleBy_ID(string ID);
    }
}