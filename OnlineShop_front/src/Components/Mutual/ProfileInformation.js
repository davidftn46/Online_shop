import React, { Fragment, useContext,useEffect,useState,useRef } from "react";
import axios from 'axios'
import {useNavigate} from 'react-router-dom' 

import Button from "../UserInterface/ButtonDesign/Button";
import classes from "./ProfileIDesign.module.css";
import Authentication from "../../Contexts/authentication";
import Card from "../UserInterface/CardDesign/Card";

const getImageType = (image) => {
  if (image.startsWith("/9j/")) {
    return "image/jpeg";
  } else if (image.startsWith("iVBORw0KGgo")) {
    return "image/png";
  } else if (image.startsWith("PHN2Zy")) {
    return "image/svg+xml";
  } else if (image.startsWith("R0lGODlh")) {
    return "image/gif";
  } else {
    return "";
  }
};

function ProfileInformation() {
  const ctx = useContext(Authentication);
  const navigate = useNavigate();

  const [firstnameIsValid, setFirstnameIsValid] = useState(true); 
  const [lastnameIsValid, setLastnameIsValid] = useState(true); 
  const [addressIsValid, setAddressIsValid] = useState(true); 
  const [dateIsValid, setDateIsValid] = useState(true); 

  const [formIsValid, setFormIsValid] = useState(false);

  const firstnameInputRef = useRef();
  const lastnameInputRef=useRef();
  const addressInputRef=useRef();
  const dateInputRef=useRef();

  const birthDate = new Date(ctx.user.BirthDate);
  const day = String(birthDate.getDate()).padStart(2, "0");
  const month = String(birthDate.getMonth() + 1).padStart(2, "0");
  const year = birthDate.getFullYear();
  const formattedDate = `${year}-${month}-${day}`;
  let role = "";
  var imageURL = "";
  if (ctx.user.Avatar != null)
    imageURL = `data:${getImageType(ctx.user.Avatar)};base64,${
      ctx.user.Avatar
    }`;

  if (ctx.user.Role == 1) role = "Customer";
  else if (ctx.user.Role == 2) role = "Seller";
  else role = "Administrator";

  useEffect(()=>{
    const button = document.getElementById('save');

  }, []);

  const firstnameChangeHandler = (event) => {
    const regex = /^(?=.*[a-zA-Z])[a-zA-Z\s]+$/;
    setFirstnameIsValid(regex.test(event.target.value));

    setFormIsValid(
    dateIsValid && addressIsValid && lastnameIsValid && firstnameIsValid
    ); 
  };

  const lastnameChangeHandler = (event) => {
    const regex = /^(?=.*[a-zA-Z])[a-zA-Z\s]+$/;
    setLastnameIsValid(regex.test(event.target.value));

    setFormIsValid(
    dateIsValid && addressIsValid && lastnameIsValid && firstnameIsValid
    ); 
  };

  const addressChangeHandler = (event) => {
    const regex = /^(?=.*[a-zA-Z])[a-zA-Z0-9\s]+$/;
    setAddressIsValid(regex.test(event.target.value));

    setFormIsValid(
    dateIsValid && addressIsValid && lastnameIsValid && firstnameIsValid
    ); 
  };

  const dateChangeHandler = (event) => {
    const inputDate = new Date(event.target.value);
    const currentDate = new Date();
    
    
    const minDate = new Date();
    minDate.setFullYear(currentDate.getFullYear() - 18);
    
    const isDateValid = inputDate <= minDate;
  
    setDateIsValid(isDateValid);
  
    setFormIsValid(
      isDateValid && addressIsValid && lastnameIsValid && firstnameIsValid
    );
  };


  const submitHandler = async (event) => {
    event.preventDefault();
    const formData = new FormData();
    if (formIsValid) {

      formData.append("UserName", ctx.user.UserName);
      formData.append("Email", ctx.user.Email);
      formData.append("FirstName", event.target.firstname.value);
      formData.append("LastName", event.target.lastname.value);
      formData.append("BirthDate", event.target.birthDate.value);
      formData.append("Address", event.target.address.value);
      if (event.target.avatar.files.length > 0)
        formData.append("file", event.target.avatar.files[0]);
      console.log(formData)
      try {
        const response = await axios.post(
          process.env.REACT_APP_SERVER_URL + "users/updateUser",
          formData,
          {
            headers: {
              "Content-Type": "multipart/form-data",
            },
          }
        );
        
        if(response.status===200)
        {
          ctx.user.FirstName=response.data.firstName;
          ctx.user.Address=response.data.address;
          ctx.user.LastName=response.data.lastName;
          ctx.user.BirthDate=response.data.birthDate;
          ctx.user.Avatar=response.data.avatar;
          ctx.onLogin(ctx.user);

          navigate('/profile');
        }
        
      } catch (error) {
        alert(error)

      }
    } 
    else if(!firstnameIsValid)
    {
      firstnameInputRef.current.focus();
    }
    else if(!lastnameIsValid)
    {
      lastnameInputRef.current.focus();
    }
    else if(!addressIsValid)
    {
      addressInputRef.current.focus();
    }
    else if(!dateIsValid)
    {
      dateInputRef.current.focus();
    }
  };

  return (
    <Card>
      <form className={classes.summary} onSubmit={submitHandler}>
        <h2>Your Profile</h2>
        <img
          className={classes.profilePic}
          src={imageURL}
          alt="Profile picture"
        />
        <br />
        <label>Chnage profile picture</label>
        <br />
        <input className={classes.input} id="avatar" type="file" />
        <p>
          Username : <b> {ctx.user.UserName}</b>
        </p>
        <br />
        <p>
          First name : <input ref={firstnameInputRef} id="firstname"  type="text" className={classes.input} defaultValue={ctx.user.FirstName} onChange={firstnameChangeHandler}/>
        </p>
        <br />
        <p>
          Last name : <input ref={lastnameInputRef} id="lastname" type="text" className={classes.input} defaultValue={ctx.user.LastName}  onChange={lastnameChangeHandler}/>
        </p>
        <br />
        <p>
          Email : <b> {ctx.user.Email}</b>
        </p>
        <br />
        <p>
          Address : <input ref={addressInputRef} id="address" type="text" className={classes.input} defaultValue={ctx.user.Address}  onChange={addressChangeHandler}/>
        </p>
        <br />
        <p>
          Birth date : <input ref={dateInputRef} id="birthDate" className={classes.input} defaultValue={formattedDate} type="date" onChange={dateChangeHandler} />
        </p>
        <br />
        <p>
          Account type : <b> {role}</b>
        </p>
        <br />
        {ctx.user.IsVerified ? (
          <p>
            Account Status :<b> Verified</b>
          </p>
        ) : (
          <p>
            Account Status : <b>Pending</b>
          </p>
        )}
        <center>
          <Button type="submit" id='save'> Save changes</Button>
        </center>
      </form>
    </Card>
  );
}

export default ProfileInformation;