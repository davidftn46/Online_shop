import React,{useEffect, Fragment,useContext} from 'react'
import { v4 as uuidv4 } from 'uuid';

import classes from './OrderHistoryDesign.module.css'

import Card from '../../UserInterface/CardDesign/Card'
import OrderCard from '../OrderCard'
import OrderContext from '../../../Contexts/order-context';

const OrderHistory = () => {
  const ctx = useContext(OrderContext);

  useEffect(() => {
    ctx.onFetchHistory();
  });

  return (
    <Fragment>
    <section className={classes.summary}>
      <h2>My orders</h2>

      <section className={classes.users}>
        {ctx.orderHistory.length > 0 ? (
          <Card>
            <ul>
              {ctx.orderHistory.map((order) => (
                <OrderCard
                  key={uuidv4()}
                  id={order.id}
                  Products={order.orderProducts}
                />
              ))}
            </ul>
          </Card>
        ) : (
          <h2>No-one ever bought anything from you</h2>
        )}
      </section>
    </section>
  </Fragment>
  )
}

export default OrderHistory