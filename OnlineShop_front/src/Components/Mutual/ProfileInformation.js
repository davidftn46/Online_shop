import React, { useContext } from 'react';

import classes from './ProfileIDesign.module.css'
import Card from '../UserInterface/CardDesign/Card';
import Authentication from '../../Contexts/authentication';

function ProfileInfo() {
  const ctx = useContext(Authentication)
  return (
    <Card>
      <div>David</div>
    </Card>
  );
}

export default ProfileInfo;