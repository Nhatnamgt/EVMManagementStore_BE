using PlanyApp.Repository.Base;
using EVMManagementStore.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Repository.UnitOfWork
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly EVMManagementStoreContext _context;

        private GenericRepository<DealerOrder> _dealerorderRepository = null!;
        private GenericRepository<Inventory> _inventoryRepository = null!;
        private GenericRepository<Order> _orderRepository = null!;
        private GenericRepository<Payment> _paymentRepository = null!;
        private GenericRepository<Promotion> _promotionRepository = null!;
        private GenericRepository<PromotionOption> _promotionoptionRepository = null!;
        private GenericRepository<Quotation> _quotiationRepository = null!;
        private GenericRepository<Report> _reportRepository = null!;
        private GenericRepository<Role> _roleRepository = null!;
        private GenericRepository<SalesContract> _salecontractRepository = null!;
        private GenericRepository<TestDriveAppointment> _testdriveappointmentRepository = null!;
        private GenericRepository<Vehicle> _vehicleRepository = null!;
        private GenericRepository<User> _userRepository = null!;
        private GenericRepository<Delivery> _deliveryRepository = null!;

        public UnitOfWork(EVMManagementStoreContext context)
        {
            _context = context;
        }

        public GenericRepository<DealerOrder> DealerOrderRepository => _dealerorderRepository ??= new GenericRepository<DealerOrder>(_context);
        public GenericRepository<Inventory> InventoryRepository => _inventoryRepository ??= new GenericRepository<Inventory>(_context);
        public GenericRepository<Order> OrderRepository => _orderRepository ??= new GenericRepository<Order>(_context);
        public GenericRepository<Payment> PaymentRepository => _paymentRepository ??= new GenericRepository<Payment>(_context);
        public GenericRepository<Promotion> PromotionRepository => _promotionRepository ??= new GenericRepository<Promotion>(_context);
        public GenericRepository<PromotionOption> PromotionOptionRepository => _promotionoptionRepository ??= new GenericRepository<PromotionOption>(_context);
        public GenericRepository<Quotation> QuotationRepository => _quotiationRepository ??= new GenericRepository<Quotation>(_context);
        public GenericRepository<Report> ReportRepository => _reportRepository ??= new GenericRepository<Report>(_context);
        public GenericRepository<Role> RoleRepository => _roleRepository ??= new GenericRepository<Role>(_context);
        public GenericRepository<SalesContract> SalesContractRepository => _salecontractRepository ??= new GenericRepository<SalesContract>(_context);
        public GenericRepository<TestDriveAppointment> TestDriveAppointmentRepository => _testdriveappointmentRepository ??= new GenericRepository<TestDriveAppointment>(_context);
        public GenericRepository<Vehicle> VehicleRepository => _vehicleRepository ??= new GenericRepository<Vehicle>(_context);
        public GenericRepository<User> UserRepository => _userRepository ??= new GenericRepository<User>(_context);
        public GenericRepository<Delivery> DeliveryRepository => _deliveryRepository ??= new GenericRepository<Delivery>(_context);

        public int Save() => _context.SaveChanges();

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
           await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        // IDisposable
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
