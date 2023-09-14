import React from 'react'

const CartContext=React.createContext({
    products:[],
    totalProducts:0,
    addProduct:(product)=>{},
    removeProduct:(id)=>{}
});

export default CartContext;