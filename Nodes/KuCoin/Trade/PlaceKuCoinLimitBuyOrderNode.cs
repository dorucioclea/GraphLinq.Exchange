using NodeBlock.Engine;
using NodeBlock.Engine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NodeBlock.Plugin.Exchange.Nodes.KuCoin.Trade
{
    [NodeDefinition("PlaceKuCoinLimitBuyOrderNode", "Place KuCoin Limit Buy Order", NodeTypeEnum.Function, "KuCoin")]
    [NodeGraphDescription("Place KuCoin Limit Buy Order")]
    public class PlaceKuCoinLimitBuyOrderNode : Node
    {
        public PlaceKuCoinLimitBuyOrderNode(string id, BlockGraph graph)
            : base(id, graph, typeof(PlaceKuCoinLimitBuyOrderNode).Name)
        {
            this.InParameters.Add("kucoin", new NodeParameter(this, "kucoin", typeof(object), true));
            this.InParameters.Add("symbol", new NodeParameter(this, "symbol", typeof(string), true));
            this.InParameters.Add("price", new NodeParameter(this, "price", typeof(double), true));
            this.InParameters.Add("quantity", new NodeParameter(this, "quantity", typeof(double), true));

            this.OutParameters.Add("isSuccess", new NodeParameter(this, "isSuccess", typeof(bool), false));
            this.OutParameters.Add("tradeId", new NodeParameter(this, "tradeId", typeof(string), false));
            this.OutParameters.Add("error", new NodeParameter(this, "error", typeof(string), false));
        }

        public override bool CanBeExecuted => true;

        public override bool CanExecute => true;

        public override bool OnExecution()
        {
            KuCoinConnectorNode connector = this.InParameters["kucoin"].GetValue() as KuCoinConnectorNode;
            var symbol = this.InParameters["symbol"].GetValue().ToString();
            var price = Convert.ToDecimal(this.InParameters["price"].GetValue().ToString());
            var quantity = Convert.ToDecimal(this.InParameters["quantity"].GetValue().ToString());

            var result = connector.Client.PlaceOrderAsync(symbol, Kucoin.Net.Objects.KucoinOrderSide.Buy, Kucoin.Net.Objects.KucoinNewOrderType.Limit, price, quantity);

            if(!result.Result.Success)
            {
                this.OutParameters["isSuccess"].SetValue(bool.Parse("false"));
                this.OutParameters["tradeId"].SetValue("");
                this.OutParameters["error"].SetValue(result.Result.Error);

                return true;
            }

            this.OutParameters["isSuccess"].SetValue(bool.Parse("true"));
            this.OutParameters["tradeId"].SetValue(result.Result.Data.OrderId.ToString());
            this.OutParameters["error"].SetValue(result.Result.Error);

            return true;
        }
    }
}
