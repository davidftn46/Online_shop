import React,{useState,useEffect} from 'react'
import axios from 'axios';

import Product from '../Models/Product'

const ProductContext = React.createContext({
    onFetch: ()=>{},
});

export const ProductContextBuying = (props) => {
    const [products,setProducts] = useState([]);

    const fetchHandler=(products)=>{
        axios.get(process.env.REACT_APP_SERVER_URL+'products/allProducts')
        .then(response => {
        if(response.data != null){
            setProducts(response.data.map(element => new Product(element)));
        }
        else
        setProducts([])
        });
    }

    return (
        <ProductContext.Provider
        value={{
            products:products,
            onFetch: fetchHandler, 
            }}>
            {props.children}
        </ProductContext.Provider>
    )

}

export default ProductContext;