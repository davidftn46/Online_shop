import React, { Fragment, useEffect, useContext } from "react";
import { v4 as uuidv4 } from "uuid";

import classes from "./MyOrdersDesign.module.css";

import OrderContext from "../../../Contexts/order-context";
import Card from "../../UserInterface/CardDesign/Card";
import OrderCard from "../OrderCard";

const MyOrders = () => {
  const ctx = useContext(OrderContext);

  useEffect(() => {
    ctx.onFetchCustomers();
  },[]);

  return (
    <Fragment>
      <section className={classes.summary}>
        <h2>My orders</h2>

        <section className={classes.users}>
          {ctx.customersOrders.length > 0 ? (
            <Card>
              <ul>
                {ctx.customersOrders.map((order) => (
                  <OrderCard
                    key={uuidv4()}
                    id={order.id}
                    Products={order.orderProducts}
                    shipped={order.shipped}
                    canceled={order.canceled}
                    minutes={order.minutes}
                  />
                ))}
              </ul>
            </Card>
          ) : (
            <h2>You never ordered anything</h2>
          )}
        </section>
      </section>
    </Fragment>
  );
};

export default MyOrders;