using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVMManagementStore.Service.Service.Dealer
{
    public class SaleManagementService : ISaleManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        public SaleManagementService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        //=======================================================Quotation===========================================================
        public async Task<List<QuotationDTO>> GetAllQuotationsAsync()
        {
            var quotations = await _unitOfWork.QuotationRepository.GetAllAsync();
            return quotations.Select(q => new QuotationDTO
            {
                QuotationId = q.QuotationId,
                UserId = q.UserId,
                VehicleId = q.VehicleId,
                QuotationDate = q.QuotationDate,
                BasePrice = q.BasePrice,
                Discount = q.Discount,
                FinalPrice = q.FinalPrice,
                AttachmentFile = q.AttachmentFile,
                AttachmentImage = q.AttachmentImage,    
                Status = q.Status
            }).ToList();
        }
        public async Task<QuotationDTO> UploadFiles(int quotationId, IFormFile attachmentFile, IFormFile attachmentImage)
        {
            // Lấy quotation
            var quotation = await _unitOfWork.QuotationRepository.GetByIdAsync(quotationId);
            if (quotation == null)
                throw new Exception("Quotation not found");

            // Tạo thư mục uploads nếu chưa có
            string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Lưu file đính kèm (hợp đồng)
            if (attachmentFile != null)
            {
                string fileName = $"{quotationId}_contract_{Path.GetFileName(attachmentFile.FileName)}";
                string filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachmentFile.CopyToAsync(stream);
                }
                quotation.AttachmentFile = $"uploads/{fileName}";
            }

            // Lưu file hình ảnh (hóa đơn)
            if (attachmentImage != null)
            {
                string imageName = $"{quotationId}_image_{Path.GetFileName(attachmentImage.FileName)}";
                string imagePath = Path.Combine(uploadPath, imageName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await attachmentImage.CopyToAsync(stream);
                }
                quotation.AttachmentImage = $"uploads/{imageName}";
            }

            // Lưu cập nhật vào database
            _unitOfWork.QuotationRepository.Update(quotation);
            await _unitOfWork.SaveAsync();

            // Trả về DTO
            return new QuotationDTO
            {
                QuotationId = quotation.QuotationId,
                UserId = quotation.UserId,
                VehicleId = quotation.VehicleId,
                QuotationDate = quotation.QuotationDate,
                BasePrice = quotation.BasePrice,
                Discount = quotation.Discount,
                FinalPrice = quotation.FinalPrice,
                AttachmentFile = quotation.AttachmentFile,
                AttachmentImage = quotation.AttachmentImage,
                Status = quotation.Status
            };
        }
        public async Task<QuotationDTO> GetQuotationByIdAsync(int id)
        {
            var q = await _unitOfWork.QuotationRepository.GetByIdAsync(id);
            if (q == null)
                throw new KeyNotFoundException($"QuotationId {id} not found.");

            return new QuotationDTO
            {
                QuotationId = q.QuotationId,
                UserId = q.UserId,
                VehicleId = q.VehicleId,
                QuotationDate = q.QuotationDate,
                BasePrice = q.BasePrice,
                Discount = q.Discount,
                FinalPrice = q.FinalPrice,
                AttachmentFile = q.AttachmentFile,
                AttachmentImage = q.AttachmentImage,
                Status = q.Status
            };
        }
        public async Task<QuotationDTO> CreateQuotationAsync(QuotationDTO quotationDTO)
        {
            if (quotationDTO == null)
                throw new ArgumentNullException(nameof(quotationDTO));

            var quotation = new Quotation
            {
                UserId = quotationDTO.UserId,
                VehicleId = quotationDTO.VehicleId,
                QuotationDate = quotationDTO.QuotationDate ?? DateTime.UtcNow,
                BasePrice = quotationDTO.BasePrice,
                Discount = quotationDTO.Discount,
                AttachmentFile = quotationDTO.AttachmentFile,   
                AttachmentImage = quotationDTO.AttachmentImage, 
                Status = string.IsNullOrEmpty(quotationDTO.Status) ? "Pending" : quotationDTO.Status
            };

            decimal discountValue = quotation.Discount ?? 0;
            quotation.FinalPrice = quotation.BasePrice - (quotation.BasePrice * discountValue);

            await _unitOfWork.QuotationRepository.AddAsync(quotation);
            await _unitOfWork.SaveAsync();

            return new QuotationDTO
            {
                QuotationId = quotation.QuotationId,
                UserId = quotation.UserId,
                VehicleId = quotation.VehicleId,
                QuotationDate = quotation.QuotationDate,
                BasePrice = quotation.BasePrice,
                Discount = quotation.Discount,
                FinalPrice = quotation.FinalPrice,
                AttachmentFile = quotation.AttachmentFile,
                AttachmentImage = quotation.AttachmentImage,
                Status = quotation.Status
            };
        }
        public async Task<QuotationDTO> UpdateQuotationAsync(int id, QuotationDTO dto)
        {
            var q = await _unitOfWork.QuotationRepository.GetByIdAsync(id);
            if (q == null)
                throw new KeyNotFoundException($"QuotationId {id} not found.");

            q.BasePrice = dto.BasePrice;
            q.Discount = dto.Discount;
            q.Status = dto.Status;
            q.AttachmentFile = dto.AttachmentFile;
            q.AttachmentImage = dto.AttachmentImage;    
            q.FinalPrice = q.BasePrice - (q.BasePrice * (q.Discount ?? 0));

            _unitOfWork.QuotationRepository.Update(q);
            await _unitOfWork.SaveAsync();

            return new QuotationDTO
            {
                QuotationId = q.QuotationId,
                UserId = q.UserId,
                VehicleId = q.VehicleId,
                QuotationDate = q.QuotationDate,
                BasePrice = q.BasePrice,
                Discount = q.Discount,
                FinalPrice = q.FinalPrice,
                AttachmentFile = q.AttachmentFile,
                AttachmentImage = q.AttachmentImage,
                Status = q.Status
            };
        }
        public async Task<bool> DeleteQuotationAsync(int id)
        {
            var quotation = await _unitOfWork.QuotationRepository.GetByIdAsync(id);
            if (quotation == null) return false;

            await _unitOfWork.QuotationRepository.RemoveAsync(quotation);
            await _unitOfWork.SaveAsync();
            return true;
        }

        //=======================================================Order===========================================================
        public async Task<List<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync();
            return orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                QuotationId = o.QuotationId,
                UserId = o.UserId,
                VehicleId = o.VehicleId,
                OrderDate = o.OrderDate,
                DeliveryAddress = o.DeliveryAddress,    
                Status = o.Status,
                TotalAmount = o.TotalAmount
            }).ToList();
        }
        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"OrderId {id} not found.");

            return new OrderDTO
            {
                OrderId = order.OrderId,
                QuotationId = order.QuotationId,
                UserId = order.UserId,
                VehicleId = order.VehicleId,
                OrderDate = order.OrderDate,
                DeliveryAddress = order.DeliveryAddress,
                Status = order.Status,
                TotalAmount = order.TotalAmount
            };
        }
        public async Task<OrderDTO> CreateOrderAsync(OrderDTO orderDTO)
        {
            if (orderDTO == null)
                throw new ArgumentNullException(nameof(orderDTO));

            decimal totalAmount = orderDTO.TotalAmount;
            if (orderDTO.QuotationId.HasValue)
            {
                var quotation = await _unitOfWork.QuotationRepository.GetByIdAsync(orderDTO.QuotationId.Value);
                if (quotation == null)
                    throw new KeyNotFoundException($"QuotationId {orderDTO.QuotationId.Value} not found.");

                totalAmount = quotation.FinalPrice;
            }

            var order = new Order
            {
                QuotationId = orderDTO.QuotationId,
                UserId = orderDTO.UserId,
                VehicleId = orderDTO.VehicleId,
                OrderDate = orderDTO.OrderDate ?? DateTime.UtcNow,
                DeliveryAddress = orderDTO.DeliveryAddress,
                Status = string.IsNullOrEmpty(orderDTO.Status) ? "Pending" : orderDTO.Status,
                TotalAmount = totalAmount
            };

            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveAsync();

            return new OrderDTO
            {
                OrderId = order.OrderId,
                QuotationId = order.QuotationId,
                UserId = order.UserId,
                VehicleId = order.VehicleId,
                OrderDate = order.OrderDate,
                DeliveryAddress = order.DeliveryAddress,
                Status = order.Status,
                TotalAmount = order.TotalAmount
            };
        }
        public async Task<OrderDTO> UpdateOrderAsync(int id, OrderDTO dto)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"OrderId {id} not found.");
          
            order.OrderDate = dto.OrderDate;    
            order.DeliveryAddress = dto.DeliveryAddress;   
            order.Status = dto.Status;
            order.TotalAmount = dto.TotalAmount;

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return new OrderDTO
            {
                OrderId = order.OrderId,
                QuotationId = order.QuotationId,
                UserId = order.UserId,
                VehicleId = order.VehicleId,
                OrderDate = order.OrderDate,
                DeliveryAddress = order.DeliveryAddress,
                Status = order.Status,
                TotalAmount = order.TotalAmount
            };
        }
        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            if (order == null) return false;

            await _unitOfWork.OrderRepository.RemoveAsync(order);
            await _unitOfWork.SaveAsync();
            return true;
        }

        //=======================================================DealerOrder===========================================================
        public async Task<List<DealerOrderDTO>> GetAllDealerOrdersAsync()
        {
            var dealerOrders = await _unitOfWork.DealerOrderRepository.GetAllAsync();
            return dealerOrders.Select(d => new DealerOrderDTO
            {
                DealerOrderId = d.DealerOrderId,
                UserId = d.UserId,
                VehicleId = d.VehicleId,
                Quantity = d.Quantity,
                OrderDate = d.OrderDate,
                Status = d.Status,
                PaymentStatus = d.PaymentStatus,
                TotalAmount = d.TotalAmount
            }).ToList();
        }
        public async Task<DealerOrderDTO> GetDealerOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.DealerOrderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"DealerOrderId {id} not found.");

            return new DealerOrderDTO
            {
                DealerOrderId = order.DealerOrderId,
                UserId = order.UserId,
                VehicleId = order.VehicleId,
                Quantity = order.Quantity,
                OrderDate = order.OrderDate,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                TotalAmount = order.TotalAmount
            };
        }
        public async Task<DealerOrderDTO> CreateDealerOrderAsync(DealerOrderDTO dealerorderDTO)
        {
            if (dealerorderDTO == null)
                throw new ArgumentNullException(nameof(dealerorderDTO));

            var vehicle = await _unitOfWork.VehicleRepository.GetByIdAsync(dealerorderDTO.VehicleId);
            if (vehicle == null)
                throw new KeyNotFoundException($"VehicleId {dealerorderDTO.VehicleId} not found.");

            var dealerOrder = new DealerOrder
            {
                UserId = dealerorderDTO.UserId,
                VehicleId = dealerorderDTO.VehicleId,
                Quantity = dealerorderDTO.Quantity,
                OrderDate = dealerorderDTO.OrderDate ?? DateTime.UtcNow,
                Status = string.IsNullOrEmpty(dealerorderDTO.Status) ? "Pending" : dealerorderDTO.Status,
                PaymentStatus = string.IsNullOrEmpty(dealerorderDTO.PaymentStatus) ? "Unpaid" : dealerorderDTO.PaymentStatus,
                TotalAmount = dealerorderDTO.TotalAmount,
            };

            await _unitOfWork.DealerOrderRepository.AddAsync(dealerOrder);
            await _unitOfWork.SaveAsync();

            return new DealerOrderDTO
            {
                DealerOrderId = dealerOrder.DealerOrderId,
                UserId = dealerOrder.UserId,
                VehicleId = dealerOrder.VehicleId,
                Quantity = dealerOrder.Quantity,
                OrderDate = dealerOrder.OrderDate,
                Status = dealerOrder.Status,
                PaymentStatus = dealerOrder.PaymentStatus,
                TotalAmount = dealerOrder.TotalAmount
            };
        }
        public async Task<DealerOrderDTO> UpdateDealerOrderAsync(int id, DealerOrderDTO dto)
        {
            var order = await _unitOfWork.DealerOrderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"DealerOrderId {id} not found.");

            order.Quantity = dto.Quantity;
            order.Status = dto.Status;
            order.PaymentStatus = dto.PaymentStatus;
            order.TotalAmount = dto.TotalAmount;

            _unitOfWork.DealerOrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return new DealerOrderDTO
            {
                DealerOrderId = order.DealerOrderId,
                UserId = order.UserId,
                VehicleId = order.VehicleId,
                Quantity = order.Quantity,
                OrderDate = order.OrderDate,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                TotalAmount = order.TotalAmount
            };
        }
        public async Task<bool> DeleteDealerOrderAsync(int id)
        {
            var dealerOrder = await _unitOfWork.DealerOrderRepository.GetByIdAsync(id);
            if (dealerOrder == null) return false;

            await _unitOfWork.DealerOrderRepository.RemoveAsync(dealerOrder);
            await _unitOfWork.SaveAsync();
            return true;
        }

        //=======================================================SaleContract===========================================================

        public async Task<List<SalesContractDTO>> GetAllSaleContractsAsync()
        {
            var saleContracts = await _unitOfWork.SalesContractRepository.GetAllAsync();
            return saleContracts.Select(s => new SalesContractDTO
            {
                SalesContractId = s.SalesContractId,
                OrderId = s.OrderId,
                ContractDate = s.ContractDate,
                Terms = s.Terms,
                SignedByDealer = s.SignedByDealer,
                CustomerName = s.CustomerName,
                Phone = s.Phone,
                Email = s.Email,
                PaymentMethod = s.PaymentMethod,
                Address = s.Address,
                Cccd = s.Cccd,
                ContractImage = s.ContractImage
            }).ToList();
        }
        public async Task<SalesContractDTO> GetSaleContractByIdAsync(int id)
        {
            var c = await _unitOfWork.SalesContractRepository.GetByIdAsync(id);
            if (c == null)
                throw new KeyNotFoundException($"SalesContractId {id} not found.");

            return new SalesContractDTO
            {
                SalesContractId = c.SalesContractId,
                OrderId = c.OrderId,
                ContractDate = c.ContractDate,
                Terms = c.Terms,
                SignedByDealer = c.SignedByDealer,
                CustomerName = c.CustomerName,
                Phone = c.Phone,
                Email = c.Email,
                PaymentMethod = c.PaymentMethod,
                Address = c.Address,
                Cccd = c.Cccd,
                ContractImage = c.ContractImage
            };
        }
        public async Task<SalesContractDTO> CteateSaleContractAsync(SalesContractDTO salesContractDTO)
        {
            if (salesContractDTO == null)
                throw new ArgumentNullException(nameof(salesContractDTO));

            var salecontract = new SalesContract
            {
                OrderId = salesContractDTO.OrderId,
                ContractDate = salesContractDTO.ContractDate ?? DateTime.UtcNow,
                Terms = salesContractDTO.Terms,
                SignedByDealer = salesContractDTO.SignedByDealer,
                CustomerName = salesContractDTO.CustomerName,
                Phone = salesContractDTO.Phone,
                Email = salesContractDTO.Email,
                PaymentMethod = salesContractDTO.PaymentMethod,
                Address = salesContractDTO.Address,
                Cccd = salesContractDTO.Cccd,
                ContractImage = salesContractDTO.ContractImage
            };

            await _unitOfWork.SalesContractRepository.AddAsync(salecontract);
            await _unitOfWork.SaveAsync();

            return new SalesContractDTO
            {
                SalesContractId = salecontract.SalesContractId,
                OrderId = salecontract.OrderId,
                ContractDate = salecontract.ContractDate ?? DateTime.UtcNow,
                Terms = salecontract.Terms,
                SignedByDealer = salecontract.SignedByDealer,
                CustomerName = salecontract.CustomerName,
                Phone = salecontract.Phone,
                Email = salecontract.Email,
                PaymentMethod = salecontract.PaymentMethod,
                Address = salecontract.Address,
                Cccd = salecontract.Cccd,
                ContractImage = salecontract.ContractImage

            };
        }
        public async Task<SalesContractDTO> UpdateSaleContractAsync(int id, SalesContractDTO dto)
        {
            var c = await _unitOfWork.SalesContractRepository.GetByIdAsync(id);
            if (c == null)
                throw new KeyNotFoundException($"SalesContractId {id} not found.");

            c.Terms = dto.Terms;
            c.SignedByDealer = dto.SignedByDealer;
            c.CustomerName = c.CustomerName;
            c.Phone = dto.Phone;
            c.Email = dto.Email;
            c.PaymentMethod = dto.PaymentMethod;
            c.Address = dto.Address;
            c.Cccd = dto.Cccd;
            c.ContractImage = dto.ContractImage;

            _unitOfWork.SalesContractRepository.Update(c);
            await _unitOfWork.SaveAsync();

            return new SalesContractDTO
            {
                SalesContractId = c.SalesContractId,
                OrderId = c.OrderId,
                ContractDate = c.ContractDate,
                Terms = c.Terms,
                SignedByDealer = c.SignedByDealer,
                CustomerName = c.CustomerName,
                Phone = c.Phone,
                Email = c.Email,
                PaymentMethod = c.PaymentMethod,
                Address = c.Address,
                Cccd = c.Cccd,
                ContractImage = c.ContractImage
            };
        }
        public async Task<bool> DeleteSaleContractAsync(int id)
        {
            var saleContract = await _unitOfWork.SalesContractRepository.GetByIdAsync(id);
            if (saleContract == null) return false;

            await _unitOfWork.SalesContractRepository.RemoveAsync(saleContract);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
