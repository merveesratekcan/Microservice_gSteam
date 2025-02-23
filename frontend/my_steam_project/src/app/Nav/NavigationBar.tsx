'use client'

import React from 'react'
import {IoGameControllerSharp } from 'react-icons/io5'

 function NavigationBar() {
    console.log('NavigationBar')
  return (
    <header className='sticky top-o z-50 flex justify-between bg-slate-600 p-5 items-center text-white shadow-md'>
        <div className='row'>
            <div className='col'>
            <IoGameControllerSharp size={50}/> 
            </div>
        </div>
        <div>Categories</div>
        <div>Account</div>
    </header>
  )
}
export default NavigationBar
