import React, { Fragment, useEffect,useState,useContext } from 'react'

import classes from './DashboardDes.module.css'

import DashboardProduct from './DashboardProduct'
import Card from '../UserInterface/CardDesign/Card'
import ProductContext from '../../Contexts/product-context'

const Dashboard = () => {
  const ctx = useContext(ProductContext)

  useEffect(() => {
    ctx.onFetch()
  }, []);

  return (
    <Fragment>
         <section className={classes.summary}>
            <h2>All products</h2>
            {ctx.products.length > 0 ? 
            (
              <Card>
                <section className={classes.products}>
                <ul>{ctx.products.map((product) => <DashboardProduct key={product.Id} id={product.Id} name={product.Name} description={product.Description} price={product.Price} amount={product.Amount} picture={product.PictureUrl}/>)}</ul>
                </section>
              </Card>
            ): 
            (
              <h2>No products on the market</h2>
            )}
        </section>
    </Fragment>
  )
}

export default Dashboard