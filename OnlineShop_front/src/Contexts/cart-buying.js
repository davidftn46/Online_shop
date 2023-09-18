import React, { useReducer,useContext } from "react";

import CartContext from "./cart-context";
import ProductContext from './product-context'

const defaultCartState = {
  products: [],
  totalProducts: 0,
};

const cartReducer = (state, action) => {
  if (action.type === "ADD") {
    if(action.product.amount > action.product.remainingAmount){
      return {
        products: state.products,
        totalProducts: state.totalProducts ,
      };
    }
    const updatedTotalProducts = state.totalProducts + action.product.price * action.product.amount;
    
    const existingIndex = state.products.findIndex(
      (product) => product.id === action.product.id
    );

    const existingCartProduct = state.products[existingIndex];
    let updatedProducts;
    
    if(existingCartProduct){
      if(existingCartProduct.amount + action.product.amount <= action.product.remainingAmount){

      const updatedProduct =
      {
        ...existingCartProduct, 
        amount: existingCartProduct.amount+action.product.amount
      };
      
      updatedProducts = [...state.products];
      updatedProducts[existingIndex] = updatedProduct;
      }
      else
      {
        return {
          products: state.products,
          totalProducts: state.totalProducts ,
        };
      }
    }
    else
    {
      updatedProducts= state.products.concat(action.product);
    }

    return {
      products: updatedProducts,
      totalProducts: updatedTotalProducts,
    };
  }

  if(action.type === "REMOVE"){
    
    const existingIndex = state.products.findIndex(
      (product) => product.id === action.id
      );
      
    const existingCartProduct = state.products[existingIndex];
    const updatedTotalProducts = state.totalProducts - existingCartProduct.price;
    let updatedProducts;
    if(existingCartProduct.amount === 1){
      updatedProducts = state.products.filter(product => product.id !== action.id);
    }else{
      const updatedProduct = { ...existingCartProduct , amount: existingCartProduct.amount-1};
      updatedProducts=[...state.products];
      updatedProducts[existingIndex]=updatedProduct;
    }

    return {
      products: updatedProducts,
      totalProducts: updatedTotalProducts
    }
  }

  if(action.type === "REMOVEALL"){
    return {
      products: [],
      totalProducts: 0
    }
  }
  return defaultCartState;
};

const CartBuying = (props) => {
  const ctx = useContext(ProductContext)
  const [cartState, dispatchCartAction] = useReducer(
    cartReducer,
    defaultCartState
  );

  const addProductToCartHandler = (product) => {
    const i =ctx.products.filter(i => i.Id === product.id)
  
    dispatchCartAction({ type: "ADD", product: {...product, remainingAmount:i[0].Amount}});
  };

  const removeProductToCarthandler = (id) => {
    dispatchCartAction({ type: "REMOVE", id: id });
  };

  const emptyCartHandler=()=>{
    dispatchCartAction({type: "REMOVEALL"})
  }

  const cartContext = {
    products: cartState.products,
    totalProducts: cartState.totalProducts,
    addProduct: addProductToCartHandler,
    removeProduct: removeProductToCarthandler,
    emptyCart: emptyCartHandler
  };

  return (
   
    <CartContext.Provider value={cartContext}>
      {props.children}
    </CartContext.Provider>
    
  );
};

export default CartBuying;