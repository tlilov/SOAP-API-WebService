using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailSystem.com.bronto.api;

namespace EmailSystem {

    public class BrontoTranslator {
        public deliveryObject[] TranslateDelivery(Message[] messages) {
            var deliveries = new List<deliveryObject>();

            if (messages == null)
                return deliveries.ToArray();

            foreach (Message message in messages) {
                var recipients = new List<deliveryRecipientObject>();
                recipients.Add(new deliveryRecipientObject { 
                    type = message.ContactType, 
                    id = message.ContactId });

                var delivery = new deliveryObject {
                    fromName = message.FromName,
                    fromEmail = message.From,
                    messageId = message.TemplateId,
                    start = message.DateCreated,
                    startSpecified = true,
                    recipients = recipients.ToArray(),
                    replyTracking = message.ReplyTrackingEnabled,
                    replyTrackingSpecified = message.ReplyTrackingEnabled
                };

                if (message.Transactional.GetValueOrDefault(false)) {
                    delivery.type = "transactional";
                }

                if (message.Tokens != null && message.Tokens.Count > 0) {
                    delivery.fields = TranslateTokens(message.Tokens, message.MessageFormat);
                }

                deliveries.Add(delivery);
            }

            return deliveries.ToArray();
        }

        public contactObject TranslateContact(Contact contact, Dictionary<string, string> ContactFields = null) {
            var result = new contactObject();

            result.id = contact.Id;

            if (!String.IsNullOrEmpty(contact.MessagePreference)) {
                result.msgPref = contact.MessagePreference.ToLower();
            } else {
                result.msgPref = "html";
            }


            if (contact.Fields.Count > 0) {
                if (ContactFields != null) {
                    List<contactField> fields = new List<contactField>();

                    foreach (KeyValuePair<string, string> kvp in ContactFields) {
                        if (contact.Fields.ContainsKey(kvp.Value) && (contact.Fields[kvp.Value] != null)) {
                            fields.Add(new contactField { fieldId = kvp.Key, content = contact.Fields[kvp.Value] });
                        }
                    }

                    result.fields = fields.ToArray();
                }
            }

            result.status = contact.GetStatusString();

            result.email = contact.Email;

            return result;
        }

        public messageFieldObject[] TranslateTokens(IEnumerable<EmailToken> tokens, string messageType) {
            List<messageFieldObject> fields = new List<messageFieldObject>();

            foreach (EmailToken t in tokens) {
                if (!String.IsNullOrEmpty(t.TokenValue)) {
                    fields.Add(new messageFieldObject { name = t.TokenName, content = t.TokenValue, type = messageType });
                }
            }

            return fields.ToArray();
        }

        public orderObject[] TranslateOrder(OrderData orderData) {

            List<orderObject> brontoOrderItems = new List<orderObject>();

            orderObject ord = new orderObject();

            ord.id = orderData.PheOrderNumber;
            ord.email = orderData.Email;
            ord.orderDate = orderData.OrderDate;
            ord.tid = orderData.TrackingId;

            List<productObject> prods = new List<productObject>();

            foreach (OrderProduct p in orderData.Products) {
                prods.Add(TranslateOrderProduct(p));
            }

            ord.products = prods.ToArray();

            brontoOrderItems.Add(ord);
            return brontoOrderItems.ToArray();

        }

        public productObject TranslateOrderProduct(OrderProduct orderProduct) {
            var prod = new productObject();

            prod.sku = orderProduct.Sku;
            prod.name = orderProduct.Name;
            prod.description = orderProduct.Description;
            prod.category = orderProduct.Category;
            prod.quantity = orderProduct.Quantity;
            prod.quantitySpecified = true;
            prod.price = orderProduct.UnitPrice;
            prod.priceSpecified = true;

            return prod;
        }
    }
}