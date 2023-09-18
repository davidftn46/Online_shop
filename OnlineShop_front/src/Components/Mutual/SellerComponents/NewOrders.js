import React,{useEffect, Fragment,useContext} from 'react'
import { v4 as uuidv4 } from 'uuid';

import classes from './NewOrdersDesign.module.css'
import Card from '../../UserInterface/CardDesign/Card'
import OrderCard from '../OrderCard';

import OrderContext from '../../../Contexts/order-context'
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import Authentication from '../../../Contexts/authentication';

const NewOrders = () => {
  const ctx = useContext(OrderContext);
  const authCtx = useContext(Authentication)
  const navigate = useNavigate(); 
  useEffect(() => {
    ctx.onFetchNew();
  }, []);

  const SendHandler =(event) =>{
    try{
      const response = axios.post(process.env.REACT_APP_SERVER_URL+'orders/sendProduct?id='+event.target.id,{
        headers: {
          Authorization: `Bearer ${authCtx.user.Token}`
        }
      });

      if(response.data)
        navigate('/')
    }
    catch (error){
      alert(error);
    }
  }

  return (
    <Fragment>
    <section className={classes.summary}>
      <h2>New orders</h2>

      <section className={classes.users}>
        {ctx.newOrders.length > 0 ? (
          <Card>
            <ul>
              {ctx.newOrders.map((order) => (
                <>
                <OrderCard
                  key={uuidv4()}
                  id={order.id}
                  Products={order.orderProducts}
                  shipped={order.shipped}
                    canceled={order.canceled}
                    minutes={order.minutes}
                />
                <button id={order.id} onClick={SendHandler}>Send</button>
                </>
              ))}
            </ul>
          </Card>
        ) : (
          <h2>You have no new orders</h2>
        )}
      </section>
    </section>
  </Fragment>
  )
}

export default NewOrders