import React,{useState, useContext} from 'react'
import axios from 'axios';
import Authentication from './authentication'

const OrderContext = React.createContext({
    onFetch: ()=>{},
    onFetchNew :()=>{},
    onFetchHistory:()=>{}, 
    onFetchCustomers: ()=>{},
});

export const OrderContextBuying = (props) => {
    const [allOrders,setAllOrders] = useState([]);
    const [customersOrders,setCustomersOrders] = useState([]);
    const [orderHistory,setOrderHistory] = useState([]);
    const [newOrders,setNewOrders] = useState([]);
    const ctx = useContext(Authentication)

    
    const fetchAllHandler=()=>{
        axios.get(process.env.REACT_APP_SERVER_URL+'orders/allOrders',{
            headers: {
              Authorization: `Bearer ${ctx.user.Token}`
            }
          })
        .then(response => {
        if(response.data != null){
            setAllOrders(response.data);
        }
        else
        setAllOrders([])
        });
    }

    
    const fetchNewHandler=()=>{
        axios.get(process.env.REACT_APP_SERVER_URL+'orders/newOrders?id='+ctx.user.Id, {
            headers: {
              Authorization: `Bearer ${ctx.user.Token}`
            }
          })
        .then(response => {
        if(response.data != null){
          setNewOrders(response.data);
        }
        else
        setNewOrders([])
        });
    }

    
    const fetchHistoryHandler=()=>{
        axios.get(process.env.REACT_APP_SERVER_URL+'orders/orderHistory?id='+ctx.user.Id,{
            headers: {
              Authorization: `Bearer ${ctx.user.Token}`
            }
          })
        .then(response => {
        if(response.data != null){
            setOrderHistory(response.data);
        }
        else
        setOrderHistory([])
        });
    }

    
    const fetchCustomersHandler=()=>{
        axios.get(process.env.REACT_APP_SERVER_URL+'orders/myOrders?id='+ctx.user.Id,{
            headers: {
              Authorization: `Bearer ${ctx.user.Token}`
            }
          })
        .then(response => {
            if(response.data != null){
                setCustomersOrders(response.data);
            }
            else
            setCustomersOrders([])
        });
    }

    return (
        <OrderContext.Provider
        value={{
            allOrders:allOrders,
            customersOrders:customersOrders,
            orderHistory:orderHistory,
            newOrders:newOrders,
            onFetchAll: fetchAllHandler, 
            onFetchNew: fetchNewHandler, 
            onFetchHistory: fetchHistoryHandler, 
            onFetchCustomers: fetchCustomersHandler, 
            }}>
            {props.children}
        </OrderContext.Provider>
    )

}

export default OrderContext;