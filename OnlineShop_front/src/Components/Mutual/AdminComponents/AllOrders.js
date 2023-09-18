import React,{useContext,useEffect,Fragment} from 'react'
import { v4 as uuidv4 } from 'uuid';

import classes from './AllOrdersDesign.module.css'

import OrderContext from '../../../Contexts/order-context'

import Card from '../../UserInterface/CardDesign/Card'
import OrderCard from '../OrderCard'

const AllOrders = () => {
  const ctx = useContext(OrderContext);

  useEffect(() => {
     ctx.onFetchAll();
  }, []);

  return (
    <Fragment>
    <section className={classes.summary}>
      <h2>All orders from users</h2>

    <section className={classes.users}>
        {ctx.allOrders.length > 0 ? 
        (
        <Card>
          <ul>
          {ctx.allOrders.map(order => <OrderCard key={uuidv4()} id={order.id} Customer = {order.username} Products = {order.orderProducts} />)}
          </ul>
        </Card>
        )
        : 
        (
          <h2>All users are verified</h2>
        )}
        
    </section>

    </section>
  </Fragment>
  )
}

export default AllOrders