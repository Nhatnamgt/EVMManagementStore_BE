using EVMManagementStore.Repository.Models;
using EVMManagementStore.Repository.UnitOfWork;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
                FinalPrice = q.FinalPrice,
                AttachmentFile = q.AttachmentFile,
                AttachmentImage = q.AttachmentImage,    
                Status = q.Status
            }).ToList();
        }
        public async Task<QuotationDTO> UploadFiles(int quotationId, IFormFile attachmentFile, IFormFile attachmentImage)
        {
            var quotation = await _unitOfWork.QuotationRepository.GetByIdAsync(quotationId);
            if (quotation == null)
                throw new Exception("Quotation not found");

            string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

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

            _unitOfWork.QuotationRepository.Update(quotation);
            await _unitOfWork.SaveAsync();

            return new QuotationDTO
            {
                QuotationId = quotation.QuotationId,
                UserId = quotation.UserId,
                VehicleId = quotation.VehicleId,
                QuotationDate = quotation.QuotationDate,
                BasePrice = quotation.BasePrice,
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
                Status = string.IsNullOrEmpty(quotationDTO.Status) ? "Pending" : quotationDTO.Status
            };

            await _unitOfWork.QuotationRepository.AddAsync(quotation);
            await _unitOfWork.SaveAsync();

            return new QuotationDTO
            {
                QuotationId = quotation.QuotationId,
                UserId = quotation.UserId,
                VehicleId = quotation.VehicleId,
                QuotationDate = quotation.QuotationDate,
                BasePrice = quotation.BasePrice,
                FinalPrice = quotation.FinalPrice,
                Status = quotation.Status
            };
        }
        public async Task<QuotationDTO> UpdateQuotationAsync(int id, QuotationDTO dto)
        {
            var q = await _unitOfWork.QuotationRepository.GetByIdAsync(id);
            if (q == null)
                throw new KeyNotFoundException($"QuotationId {id} not found.");

            q.BasePrice = dto.BasePrice;
            q.QuotationDate = dto.QuotationDate;    
            q.Status = dto.Status;   

            _unitOfWork.QuotationRepository.Update(q);
            await _unitOfWork.SaveAsync();

            return new QuotationDTO
            {
                QuotationId = q.QuotationId,
                UserId = q.UserId,
                VehicleId = q.VehicleId,
                QuotationDate = q.QuotationDate,
                BasePrice = q.BasePrice,
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
        public async Task<OrderDTO> UploadFilesOrder(int orderid, IFormFile attachmentFile, IFormFile attachmentImage)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderid);
            if (order == null)
                throw new Exception("Order not found");

            string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            if (attachmentFile != null)
            {
                string fileName = $"{orderid}_contract_{Path.GetFileName(attachmentFile.FileName)}";
                string filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachmentFile.CopyToAsync(stream);
                }
                order.AttachmentFile = $"uploads/{fileName}";
            }

            if (attachmentImage != null)
            {
                string imageName = $"{orderid}_image_{Path.GetFileName(attachmentImage.FileName)}";
                string imagePath = Path.Combine(uploadPath, imageName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await attachmentImage.CopyToAsync(stream);
                }
                order.AttachmentImage = $"uploads/{imageName}";
            }

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
                AttachmentFile = order.AttachmentFile,
                AttachmentImage = order.AttachmentImage,
                PromotionCode = order.PromotionCode,
                Status = order.Status,
                TotalAmount = order.TotalAmount 
            };
        }
        public async Task<List<OrderDTO>> GetAllOrdersAsync()
        {          
            var orders = await _unitOfWork.OrderRepository.GetAllIncludeAsync(p => p.Quotation);

            return orders.Select(o => new OrderDTO
                             {
                                 OrderId = o.OrderId,
                                 QuotationId = o.QuotationId,
                                 UserId = o.UserId,
                                 VehicleId = o.VehicleId,
                                 OrderDate = o.OrderDate,
                                 DeliveryAddress = o.DeliveryAddress,
                                 AttachmentFile = o.AttachmentFile,
                                 AttachmentImage = o.AttachmentImage,
                                 PromotionCode = o.PromotionCode,
                                 PromotionOptionName = o.PromotionCode,
                                 Status = o.Status,
                                 TotalAmount = o.Quotation != null ? o.Quotation.FinalPrice : 0
            }).ToList();
        }
        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var order = (await _unitOfWork.OrderRepository.FindIncludeAsync(p => p.OrderId == id, p => p.Quotation)).FirstOrDefault();

            if (order == null) return null;

            return new OrderDTO
            {
                OrderId = order.OrderId,
                QuotationId = order.QuotationId,
                UserId = order.UserId,
                VehicleId = order.VehicleId,
                OrderDate = order.OrderDate,
                DeliveryAddress = order.DeliveryAddress,
                AttachmentFile = order.AttachmentFile,
                AttachmentImage = order.AttachmentImage,
                PromotionCode = order.PromotionCode,
                PromotionOptionName = order.PromotionCode,   
                Status = order.Status,
                TotalAmount = order.Quotation != null ? order.Quotation.FinalPrice : 0
            };
        }

        public async Task<OrderDTO> CreateOrderAsync(OrderDTO orderDTO)
        {
            var quotation = await _unitOfWork.QuotationRepository.GetByIdAsync(orderDTO.QuotationId); 
            if (quotation == null) return null;

            var order = new Order
            {
                QuotationId = orderDTO.QuotationId,
                UserId = orderDTO.UserId,
                VehicleId = orderDTO.VehicleId,
                OrderDate = orderDTO.OrderDate ?? DateTime.UtcNow,
                DeliveryAddress = orderDTO.DeliveryAddress,
                PromotionCode = orderDTO.PromotionCode,
                Status = string.IsNullOrEmpty(orderDTO.Status) ? "Pending" : orderDTO.Status,
                TotalAmount = quotation.FinalPrice
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
                PromotionCode = order.PromotionCode,
                PromotionOptionName = order.PromotionCode,  
                Status = order.Status,
                TotalAmount = order.TotalAmount
            };
        }
        public async Task<OrderDTO> UpdateOrderAsync(int id, OrderDTO dto)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id);
            if (order == null) return null;

            var quotation = await _unitOfWork.QuotationRepository.GetByIdAsync(dto.QuotationId);
            if (quotation == null) return null; 

            order.OrderDate = dto.OrderDate ?? order.OrderDate;
            order.DeliveryAddress = dto.DeliveryAddress;
            order.PromotionCode = dto.PromotionCode;
            order.TotalAmount = quotation.FinalPrice;

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
                AttachmentFile = order.AttachmentFile,
                AttachmentImage = order.AttachmentImage,
                PromotionCode = order.PromotionCode,
                PromotionOptionName = order.PromotionCode, 
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
                Color = d.Color,    
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
                Color = order.Color,    
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
                Color = dealerorderDTO.Color,       
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
                Color = dealerOrder.Color,      
                Status = dealerOrder.Status,
                PaymentStatus = dealerOrder.PaymentStatus,
                TotalAmount = dealerOrder.TotalAmount
            };
        }
        public async Task<DealerOrderDTO> UpdateDealerOrderAsync(int id, DealerOrderDTO dto)
        {
            var order = await _unitOfWork.DealerOrderRepository.GetByIdAsync(id);
            if (order == null) return null;

            order.Quantity = dto.Quantity;
            order.Status = dto.Status;
            order.Color = dto.Color;    
            order.PaymentStatus = dto.PaymentStatus;
            order.TotalAmount = dto.TotalAmount;
            order.OrderDate = dto.OrderDate;    

            _unitOfWork.DealerOrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return new DealerOrderDTO
            {
                DealerOrderId = order.DealerOrderId,
                UserId = order.UserId,
                VehicleId = order.VehicleId,
                Quantity = order.Quantity,
                OrderDate = order.OrderDate,
                Color = order.Color,    
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
        public async Task<SalesContractDTO> UploadFilesSaleContract(int salecontractid, IFormFile attachmentFile, IFormFile attachmentImage)
        {
            var salecontract = await _unitOfWork.SalesContractRepository.GetByIdAsync(salecontractid);
            if (salecontract == null)
                throw new Exception("Order not found");

            string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            if (attachmentFile != null)
            {
                string fileName = $"{salecontractid}_contract_{Path.GetFileName(attachmentFile.FileName)}";
                string filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachmentFile.CopyToAsync(stream);
                }
                salecontract.ContractFile = $"uploads/{fileName}";
            }

            if (attachmentImage != null)
            {
                string imageName = $"{salecontractid}_image_{Path.GetFileName(attachmentImage.FileName)}";
                string imagePath = Path.Combine(uploadPath, imageName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await attachmentImage.CopyToAsync(stream);
                }
                salecontract.ContractImage = $"uploads/{imageName}";
            }

            _unitOfWork.SalesContractRepository.Update(salecontract);
            await _unitOfWork.SaveAsync();

            return new SalesContractDTO
            {
                SalesContractId = salecontract.SalesContractId,
                UserId = salecontract.UserId,   
                OrderId = salecontract.OrderId,
                ContractDate = salecontract.ContractDate,
                Terms = salecontract.Terms,
                SignedByDealer = salecontract.SignedByDealer,
                PaymentMethod = salecontract.PaymentMethod,
                Cccd = salecontract.Cccd,
                ContractFile = salecontract.ContractFile, 
                ContractImage = salecontract.ContractImage
            };
        }
        public async Task<List<SalesContractDTO>> GetAllSaleContractsAsync()
        {
            var saleContracts = await _unitOfWork.SalesContractRepository.GetAllAsync();
            return saleContracts.Select(s => new SalesContractDTO
            {
                SalesContractId = s.SalesContractId,
                UserId = s.UserId,
                OrderId = s.OrderId,
                ContractDate = s.ContractDate,
                Terms = s.Terms,
                SignedByDealer = s.SignedByDealer,
                PaymentMethod = s.PaymentMethod,
                Cccd = s.Cccd,
                ContractFile = s.ContractFile,
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
                UserId = c.UserId,
                OrderId = c.OrderId,
                ContractDate = c.ContractDate,
                Terms = c.Terms,
                SignedByDealer = c.SignedByDealer,
                PaymentMethod = c.PaymentMethod,
                Cccd = c.Cccd,
                ContractFile = c.ContractFile,
                ContractImage = c.ContractImage
            };
        }
        public async Task<SalesContractDTO> CteateSaleContractAsync(SalesContractDTO salesContractDTO)
        {
            if (salesContractDTO == null)
                throw new ArgumentNullException(nameof(salesContractDTO));

            var salecontract = new SalesContract
            {
                UserId = salesContractDTO.UserId,
                OrderId = salesContractDTO.OrderId,
                ContractDate = salesContractDTO.ContractDate ?? DateTime.UtcNow,
                Terms = salesContractDTO.Terms,
                SignedByDealer = salesContractDTO.SignedByDealer,
                PaymentMethod = salesContractDTO.PaymentMethod,
                Cccd = salesContractDTO.Cccd,
            };

            await _unitOfWork.SalesContractRepository.AddAsync(salecontract);
            await _unitOfWork.SaveAsync();

            return new SalesContractDTO
            {
                SalesContractId = salecontract.SalesContractId,
                UserId = salecontract.UserId,
                OrderId = salecontract.OrderId,
                ContractDate = salecontract.ContractDate ?? DateTime.UtcNow,
                Terms = salecontract.Terms,
                SignedByDealer = salecontract.SignedByDealer,
                PaymentMethod = salecontract.PaymentMethod,
                Cccd = salecontract.Cccd,
            };
        }
        public async Task<SalesContractDTO> UpdateSaleContractAsync(int id, SalesContractDTO dto)
        {
            var c = await _unitOfWork.SalesContractRepository.GetByIdAsync(id);
            if (c == null)
                throw new KeyNotFoundException($"SalesContractId {id} not found.");

            c.Terms = dto.Terms;
            c.SignedByDealer = dto.SignedByDealer;
            c.PaymentMethod = dto.PaymentMethod;
            c.Cccd = dto.Cccd;

            _unitOfWork.SalesContractRepository.Update(c);
            await _unitOfWork.SaveAsync();

            return new SalesContractDTO
            {
                SalesContractId = c.SalesContractId,
                UserId = c.UserId,
                OrderId = c.OrderId,
                ContractDate = c.ContractDate,
                Terms = c.Terms,
                SignedByDealer = c.SignedByDealer,
                PaymentMethod = c.PaymentMethod,
                Cccd = c.Cccd,
                ContractFile = c.ContractFile,
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
