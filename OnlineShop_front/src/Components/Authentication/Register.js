import React,{useState ,useEffect, useContext,useRef }  from "react";
import axios from 'axios';
import {GoogleLogin} from '@react-oauth/google'

import classes from "./FormDesign.module.css";
import Modal from "../UserInterface/ModalDesign/Modal";
import Authentication from "../../Contexts/authentication";
import Input from '../UserInterface/InputDesign/Input'
import Button from "../UserInterface/ButtonDesign/Button";

const Register = (props) => {
    const [isFormVisible, setIsFormVisible] = useState(false);
    const [isGoogleFormVisible, setIsGoogleFormVisible] = useState(false);
    const [enteredEmail, setEnteredEmail] = useState('');
    const [enteredPassword, setEnteredPassword] = useState('');
    const [enteredPasswordRepeat, setEnteredPasswordRepeat] = useState('');
    
    const [emailIsValid, setEmailIsValid] = useState(); 
    const [passwordIsValid, setPasswordIsValid] = useState(); 
    const [passwordRepeatIsValid, setPasswordRepeatIsValid] = useState(); 

    const [formIsValid, setFormIsValid] = useState(false);

    const authCtx = useContext(Authentication);
  
    const emailInputRef = useRef();
    const passwordInputRef=useRef();
    const passwordRepeatInputRef=useRef();
  
    useEffect(()=>{
      const identifier = setTimeout(()=>{
        setFormIsValid(
          emailIsValid && passwordIsValid && passwordRepeatIsValid
        );
      },500);
      
      return ()=>{
        clearTimeout(identifier);
      };
    }, [emailIsValid,passwordIsValid,passwordRepeatIsValid]);
  
    const toggleFormVisibility = () => {
      setIsFormVisible((prevState) => !prevState);
      setIsGoogleFormVisible(false);
    };
    const toggleGoogleFormVisibility = () => {
      setIsGoogleFormVisible((prevState) => !prevState);
      setIsFormVisible(false);
    };
  
    const emailChangeHandler = (event) => {
        setEnteredEmail(event.target.value);
        const regex = /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i;
        setEmailIsValid(regex.test(event.target.value));
        setFormIsValid(
        event.target.value.includes('@') && passwordIsValid && passwordRepeatIsValid
        ); 
    };
  
    const passwordChangeHandler = (event) => {
        setEnteredPassword(event.target.value);
        
        const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
        setPasswordIsValid(regex.test(event.target.value));

        setFormIsValid(
        emailIsValid && passwordIsValid && passwordRepeatIsValid
        ); 
    };

    const passwordRepeatChangeHandler = (event) => {
        setEnteredPasswordRepeat(event.target.value);
        
        const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
        const isValid = regex.test(event.target.value) && event.target.value === enteredPassword;
        setPasswordRepeatIsValid(isValid);

        setFormIsValid(
        emailIsValid && passwordIsValid && passwordRepeatIsValid
        ); 
    
      };
  
    const submitHandler = async(event) => {
      event.preventDefault();
      const formData = new FormData();
      if(formIsValid){
        const button = document.getElementById('register');

        formData.append('UserName', event.target.username.value);
        formData.append('Email', enteredEmail);
        formData.append('FirstName', event.target.firstname.value);
        formData.append('Password', enteredPassword);
        formData.append('LastName', event.target.lastname.value);
        formData.append('BirthDate', event.target.BirthDate.value);
        formData.append('Address', event.target.address.value);
        if(event.target.avatar.files.length>0)
          formData.append('file', event.target.avatar.files[0]);
        
        let selectedOption = event.target.elements.accType.value;
        try {
          if(selectedOption === 'Customer'){
            const response = await axios.post(process.env.REACT_APP_SERVER_URL+'users/registerCustomer', formData,
            {
              headers: { 'Content-Type': 'multipart/form-data' },
            });
            alert(response.data)
            props.onClose()
          }
          else
          {
            const response = await axios.post(process.env.REACT_APP_SERVER_URL+'users/registerSeller', formData,
            {
              headers: {
                'Content-Type': 'multipart/form-data',
              },
            });
            alert(response.data)
            props.onClose()
          }
        } catch (error) {
          alert(error.response.data.detail);

        }
      }
      else if(!emailIsValid)
      {
        emailInputRef.current.focus();
      }
      else if(!passwordIsValid){
        passwordInputRef.current.focus();
      }
      else if(!passwordRepeatIsValid){
        passwordRepeatInputRef.current.focus();
      }
    };

    const registerHandler = async (response) => {
      let radio = document.getElementById('accType').value;
      let selectedOption;
      if(radio == 'Customer')
        selectedOption='customer';
      else
        selectedOption='seller'
      await axios
        .post(
          process.env.REACT_APP_SERVER_URL + "users/googleRegister",
          JSON.stringify({googleAccessToken:response.credential, role:selectedOption}),
          {
            headers: {
              "Content-Type": "application/json",
            },
          }
        )
        .then(function (apiResponse) {
          if(apiResponse.data===true)
            alert('User successfully registered');
          props.onClose();
        })
        .catch(function (error) {
          alert(error.response.data.detail);
         
        });
    };
  
  return (
    <Modal onClose={props.onClose} className={classes.login}>
    <center><h3>Register</h3>
      <button onClick={toggleFormVisibility}>
      {isFormVisible ? 'Hide Normal register' : 'Normal register'}
      </button>
    </center>
    {isFormVisible && (
      <form onSubmit={submitHandler}>
        <Input id='username' label='Username' type="text"  />
        <Input id='firstname' label='Firstname' type="text"  />
        <Input id='lastname' label='Lastname' type="text"  />
        <Input ref={emailInputRef} id='email' label='E-mail' type="email" isValid={emailIsValid} value={enteredEmail}  onChange={emailChangeHandler} />
        <Input id='address' label='Address' type="text"  />
        <Input id='BirthDate' label='Birth Date' type="date" />
        <Input ref={passwordInputRef} id='password' label='Password' type="password" isValid={passwordIsValid} value={enteredPassword}  onChange={passwordChangeHandler} />
        <Input ref={passwordRepeatInputRef} id='passwordRepeat' label='Repeat Password' type="password" isValid={passwordRepeatIsValid} value={enteredPasswordRepeat}  onChange={passwordRepeatChangeHandler} />
        <input type="radio" value="Customer" name="accType" defaultChecked /> Kupac
        <input type="radio" value="Seller" name="accType" /> Prodavac
        <Input type='file' label="Profile picture" id='avatar'/>
        <div className={classes.actions}>
          <Button type="submit" id='register' className={classes.btn} >
            Register
          </Button>

         
          
        </div>
      </form>
    )}
      <hr/>
      <center>
        <button onClick={toggleGoogleFormVisibility}>
        {isGoogleFormVisible ? 'Hide Google register' : 'Google register'}
        </button>
      </center>
      {isGoogleFormVisible && (
      <center>
        <div id="accType">
        <input type="radio" value="Customer" name="accType" defaultChecked /> Kupac
        <input type="radio" value="Seller" name="accType" /> Prodavac
        </div>
        <GoogleLogin onSuccess={registerHandler} /> 
      </center>
      )}
    </Modal>
  );
};

export default Register;