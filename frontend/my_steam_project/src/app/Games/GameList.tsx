import React from 'react'

async function GetData() {
  const response = await fetch('http://localhost:6001/game');
  if (!response.ok) {
    throw new Error('Network response was not ok');
  }
  return response.json();
    
}
async function GameList() {

    const data = await GetData();
  return (
    <div>
        {JSON.stringify(data)}
    </div>
  )
}

export default GameList
