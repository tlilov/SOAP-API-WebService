using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSystem.Tests {

    [TestClass]
    public class BrontoTranslatorTests {
        private BrontoTranslator translator;

        [TestInitialize]
        public void Init() {
            translator = new BrontoTranslator();
        }

        [TestMethod]
        public void TranslateDelivery_WhenMessagesIsNull_ReturnsEmptyList() {
            var result = translator.TranslateDelivery(null);

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void TranslateDelivery_WhenMessagesExist_ReturnsProperTranslation() {
            var message = new Message() {
                    ContactType = "contactType",
                    ContactId = "contactId",
                    FromName = "fromName",
                    From = "from",
                    TemplateId = "templateId",
                    DateCreated = DateTime.Now,
                    ReplyTrackingEnabled = true
            };

            var messages = new List<Message>() {
                message
            }.ToArray();

            var result = translator.TranslateDelivery(messages);

            Assert.AreEqual(message.ContactType, result[0].recipients[0].type);
            Assert.AreEqual(message.ContactId, result[0].recipients[0].id);
            Assert.AreEqual(message.FromName, result[0].fromName);
            Assert.AreEqual(message.From, result[0].fromEmail);
            Assert.AreEqual(message.TemplateId, result[0].messageId);
            Assert.AreEqual(message.DateCreated, result[0].start);
            Assert.AreEqual(message.ReplyTrackingEnabled, result[0].replyTracking);
        }

        [TestMethod]
        public void TranslateDelivery_WhenMessagesExist_DefaultsStartSpecifiedToTrue() {
            var message = new Message() {
                ContactType = "contactType",
                ContactId = "contactId",
                FromName = "fromName",
                From = "from",
                TemplateId = "templateId",
                DateCreated = DateTime.Now,
                ReplyTrackingEnabled = true
            };

            var messages = new List<Message>() {
                message
            }.ToArray();

            var result = translator.TranslateDelivery(messages);

            Assert.IsTrue(result[0].startSpecified);
        }

        [TestMethod]
        public void TranslateDelivery_WhenReplyTrackingIsEnabled_SetsReplyTracking() {
            var message = new Message() {
                ContactType = "contactType",
                ContactId = "contactId",
                FromName = "fromName",
                From = "from",
                TemplateId = "templateId",
                DateCreated = DateTime.Now,
                ReplyTrackingEnabled = true
            };

            var messages = new List<Message>() {
                message
            }.ToArray();

            var result = translator.TranslateDelivery(messages);

            Assert.IsTrue(result[0].replyTracking);
        }

        [TestMethod]
        public void TranslateDelivery_WhenReplyTrackingDisabled_SetsReplyTrackingToFalse() {
            var message = new Message() {
                ContactType = "contactType",
                ContactId = "contactId",
                FromName = "fromName",
                From = "from",
                TemplateId = "templateId",
                DateCreated = DateTime.Now,
                ReplyTrackingEnabled = false
            };

            var messages = new List<Message>() {
                message
            }.ToArray();

            var result = translator.TranslateDelivery(messages);

            Assert.IsFalse(result[0].replyTracking);
        }

        [TestMethod]
        public void TranslateDelivery_WhenMessageIsTransactional_SetsTypeToTransactional() {
            var message = new Message() {
                ContactType = "contactType",
                ContactId = "contactId",
                FromName = "fromName",
                From = "from",
                TemplateId = "templateId",
                DateCreated = DateTime.Now,
                ReplyTrackingEnabled = true,
                Transactional = true
            };

            var messages = new List<Message>() {
                message
            }.ToArray();

            var result = translator.TranslateDelivery(messages);

            Assert.AreEqual("transactional", result[0].type);
        }

        [TestMethod]
        public void TranslateDelivery_WhenMessageIsTransactional_DoesNotSetType() {
            var message = new Message() {
                ContactType = "contactType",
                ContactId = "contactId",
                FromName = "fromName",
                From = "from",
                TemplateId = "templateId",
                DateCreated = DateTime.Now,
                ReplyTrackingEnabled = true,
                Transactional = false
            };

            var messages = new List<Message>() {
                message
            }.ToArray();

            var result = translator.TranslateDelivery(messages);

            Assert.IsNull(result[0].type);
        }

        [TestMethod]
        public void TranslateDelivery_WhenMessageHasTokens_TranslatesTokens() {
            var message = new Message() {
                ContactType = "contactType",
                ContactId = "contactId",
                FromName = "fromName",
                From = "from",
                TemplateId = "templateId",
                DateCreated = DateTime.Now,
                ReplyTrackingEnabled = true,
                Transactional = false,
                Tokens = new List<EmailToken>() {
                    new EmailToken() {
                        TokenName = "tokenName",
                        TokenValue = "tokenValue"
                    }
                }
            };

            var messages = new List<Message>() {
                message
            }.ToArray();

            var result = translator.TranslateDelivery(messages);

            Assert.AreEqual("tokenName", result[0].fields[0].name);
            Assert.AreEqual("tokenValue", result[0].fields[0].content);
        }

        [TestMethod]
        public void TranslateContact_WhenMessagePreferencesIsSet_SetsMessagePreference() {
            var contact = new Contact() {
                MessagePreference = "test"
            };

            var result = translator.TranslateContact(contact, null);

            Assert.AreEqual("test", result.msgPref);
        }

        [TestMethod]
        public void TranslateContact_WhenMessagePreferenceIsntSet_DefaultsToHtml() {
            var contact = new Contact();

            var result = translator.TranslateContact(contact, null);

            Assert.AreEqual("html", result.msgPref);
        }

        [TestMethod]
        public void TranslateContact_WhenCalled_SetsStatusAndEmail() {
            var contact = new Contact() {
                MessagePreference = "test",
                Status = ContactStatusEnum.Unconfirmed,
                Email = "test@test.com"
            };

            var result = translator.TranslateContact(contact, null);

            Assert.AreEqual("unconfirmed", result.status);
            Assert.AreEqual("test@test.com", result.email);
        }

        [TestMethod]
        public void TranslateContact_WhenCalledForDefaultContactObjectStatusValue_SetsStatusCorrectly()
        {
            var contact = new Contact();
            var result = translator.TranslateContact(contact, null);

            Assert.AreEqual("transactional", result.status);
        }

        [TestMethod]
        public void TranslateContact_WhenContactHasFieldsButSystemDoesNot_ReturnsNullForFields() {
            var fields = new Dictionary<string, string>();
            fields.Add("field", "value");

            var contact = new Contact() {
                MessagePreference = "test",
                Status = ContactStatusEnum.Unconfirmed,
                Email = "test@test.com",
                Fields = fields
            };

            var result = translator.TranslateContact(contact, null);

            Assert.IsNull(result.fields);
        }

        [TestMethod]
        public void TranslateContact_WhenContactHasFields_ReturnsFields() {
            var fields = new Dictionary<string, string>();
            fields.Add("field", "value");

            var contactFields = new Dictionary<string, string>();
            fields.Add("value", "here");

            var contact = new Contact() {
                MessagePreference = "test",
                Status = ContactStatusEnum.Unconfirmed,
                Email = "test@test.com",
                Fields = fields
            };

            var result = translator.TranslateContact(contact, fields);

            Assert.AreEqual("field", result.fields[0].fieldId);
            Assert.AreEqual("here", result.fields[0].content);
        }

        [TestMethod]
        public void TranslateTokens_WhenTokenDoesNotHaveValue_DoesNotGetIncludedInResult() {
            var tokens = new List<EmailToken>() {
                new EmailToken() {
                    TokenName = "first",
                    TokenValue = "set"
                },
                new EmailToken() {
                    TokenName = "second"
                }
            };

            var result = translator.TranslateTokens(tokens, "");

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("first", result[0].name);
        }

        [TestMethod]
        public void TranslateOrder_WhenCalled_TranslatesOrder() {
            var order = new OrderData() {
                PheOrderNumber = "pheOrderNumber",
                Email = "email",
                OrderDate = DateTime.Now,
                Products = new List<OrderProduct>() {
                    new OrderProduct() {
                        Category = "category",
                        Description = "description",
                        Sku = "sku",
                        Name = "name",
                        Quantity = 23,
                        UnitPrice = 1.95M
                    },
                    new OrderProduct() {
                        Category = "category",
                        Description = "description",
                        Sku = "sku",
                        Name = "name",
                        Quantity = 23,
                        UnitPrice = 1.95M
                    }
                }
            };

            var result = translator.TranslateOrder(order);

            Assert.AreEqual(order.PheOrderNumber, result[0].id);
            Assert.AreEqual(order.Email, result[0].email);
            Assert.AreEqual(order.OrderDate, result[0].orderDate);
            Assert.AreEqual(order.Products[0].Category, result[0].products[0].category);
            Assert.AreEqual(order.Products[0].Sku, result[0].products[0].sku);
            Assert.AreEqual(order.Products[0].Name, result[0].products[0].name);
            Assert.AreEqual(order.Products[0].Quantity, result[0].products[0].quantity);
            Assert.AreEqual(true, result[0].products[0].quantitySpecified);
            Assert.AreEqual(order.Products[0].UnitPrice, result[0].products[0].price);
            Assert.AreEqual(true, result[0].products[0].priceSpecified);
        }
    }
}