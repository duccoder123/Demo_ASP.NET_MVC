using MVC.DataAccess.Data;
using MVC.DataAccess.Repository.IRepository;
using MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MVC.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(OrderHeader obj)
        {
            _db.orderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderfromDb = _db.orderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderfromDb != null)
            {
                orderfromDb.OrderStatus = orderStatus;
                if(!string.IsNullOrEmpty(orderStatus))
                {
                    orderfromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderfromDb = _db.orderHeaders.FirstOrDefault(u => u.Id == id);
            if(!string.IsNullOrEmpty(sessionId))
            {
                orderfromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderfromDb.PaymentIntentId = paymentIntentId;
                orderfromDb.PaymentDay = DateTime.Now;
            }
        }
    }
}
