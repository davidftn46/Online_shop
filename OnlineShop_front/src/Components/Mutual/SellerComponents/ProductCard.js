import React from "react";

import classes from "./ProductCardDesign.module.css";
import axios from "axios";

const ProductCard = (props) => {
    const birthDate = new Date(props.BirthDate);
    const day = String(birthDate.getDate()).padStart(2, '0');
    const month = String(birthDate.getMonth() + 1).padStart(2, '0');
    const year = birthDate.getFullYear();
    const formattedDate = `${day}/${month}/${year}`;

    const VerifyHandler = async (username) =>{
        try{
            const response = await axios.post(process.env.REACT_APP_SERVER_URL+'users/verify', { 
              UserName: username,
              IsAccepted: true,
              Reason: '',
            });

            if(response.data)
              props.onVerify();
          }
          catch (error){
            console.error(error);
          }
    }

  return (
    <li className={classes.item}>
      <div>
        <h3>Name: {props.Name}</h3>
        <div className={classes.description}>
           Description: {props.Description}<br/>
           Price: {props.Price}$<br/>
           Amount: {props.Amount}<br/>
           Image: 
           {props.Image && <img src={props.Image} alt="Image" />}
        </div>
      </div>
      <div className="actions">
        <button>Modify</button>
        <button>Delete</button>
      </div>
    </li>
  )
}

export default ProductCard