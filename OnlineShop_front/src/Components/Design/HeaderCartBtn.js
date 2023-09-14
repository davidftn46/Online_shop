import React,{useContext, useEffect, useState} from 'react'
import CartIcon from '../Cart/CartIcon'

import classes from './HeaderCBD.module.css'
import CartContext from '../../Contexts/cart-context'

const HeaderCartBtn = (props) => {
  const ctx = useContext(CartContext);
  const {products} = ctx;

  const numOfProducts = products.reduce((currNumber,product)=>{
    return currNumber+product.amount;
  }, 0);

  const [btnIsHighlighted,setBtnIsHighlighted]=useState(false);

  const btnClasses= `${classes.button} ${btnIsHighlighted ? classes.bump : ''}`;

  useEffect(()=>{
    if(products.length === 0){
      return;
    }
    setBtnIsHighlighted(true);

    const timer = setTimeout(()=>{setBtnIsHighlighted(false)},300);

    return ()=>{clearTimeout(timer)};
  },[products]);

  return (
    <button className={btnClasses} onClick={props.onClick}>
        <span className={classes.icon}>
            <CartIcon/>
        </span>
        <span>Your Cart</span>
        <span className={classes.badge}>{numOfProducts}</span>
    </button>
  )
}

export default HeaderCartBtn