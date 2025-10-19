using PlanyApp.Repository.Base;
using EVMManagementStore.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Repository.UnitOfWork
{
    public interface IUnitOfWork: IDisposable
    {
        GenericRepository<DealerOrder> DealerOrderRepository { get; }
        GenericRepository<Inventory> InventoryRepository { get; }
        GenericRepository<Order> OrderRepository { get; }
        GenericRepository<Payment> PaymentRepository { get; }
        GenericRepository<Promotion> PromotionRepository { get; }
        GenericRepository<Quotation> QuotationRepository { get; }
        GenericRepository<Report> ReportRepository { get; }
        GenericRepository<Role> RoleRepository { get; }
        GenericRepository<SalesContract> SalesContractRepository { get; }
        GenericRepository<TestDriveAppointment> TestDriveAppointmentRepository { get; }
        GenericRepository<Vehicle> VehicleRepository { get; }
        GenericRepository<User> UserRepository { get; }
        GenericRepository<Delivery> DeliveryRepository { get; }
        int Save();
        Task<int> SaveAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

    }
}
