import React,{useContext} from "react";

import classes from "./OrderCardDesign.module.css";
import Authentication from '../../Contexts/authentication'

const OrderCard = (props) => {
  const ctx= useContext(Authentication)
  return (
    <li className={classes.user}>
      <div>
        <h4>Order Id: {props.id}</h4>
        {props.Customer && (<h3>Customer: {props.Customer}</h3>)}
        <div className={classes.description}>
          {props.Products.map(element => <p>Product name:<b>{element.product.name}</b> Ordered amount: <b>{element.amount}</b> Price: <b>{element.product.price}$</b></p>)}
        </div>
        <h4>Time until arrival {props.minutes}min</h4>
        {(ctx.user.Role===1 && !props.shipped) ? (
          <button >Cancel</button>
        ):
        (<>Order has arrived</>)}
        
      </div>
    </li>
  )
}

export default OrderCard