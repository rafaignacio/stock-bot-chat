import React, { useRef, useState } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';

const connection = new HubConnectionBuilder().withUrl('/chat').build();

connection.start().then(() => {
  console.log('connected');
});

connection.on('disconnected', () => {
  alert('user disconnected');
})

const sendMessage = (user, msg) => {
  connection.invoke("SendMessage", user, msg).catch( err => {
    console.log(err);
  })
}

export const Home = () => {
  const[msgs, setMessages] = useState([]);
  connection.on('ReceiveMessage', (user, msg) => {
    setMessages([...msgs, `${user} says ${msg}`]);
  });
  
  const userRef = useRef(null);
  const msgRef = useRef(null);
  
  const listItems = msgs.map((v, i) => (<li key={i}>{v}</li>));

  return (
    <section>
      <div className="container">
          <div className="row">&nbsp;</div>
          <div className="row">
              <div className="col-2">User</div>
              <div className="col-4"><input type="text" id="userInput" ref={userRef}/></div>
          </div>
          <div className="row">
              <div className="col-2">Message</div>
              <div className="col-4"><input type="text" id="messageInput" ref={msgRef}/></div>
          </div>
          <div className="row">&nbsp;</div>
          <div className="row">
              <div className="col-6">
                  <input type="button" id="sendButton" value="Send Message" onClick={() => sendMessage( userRef.current.value, msgRef.current.value )}/>
              </div>
          </div>
      </div>
      <div className="row">
          <div className="col-12">
              <hr />
          </div>
      </div>
      <div className="row">
          <div className="col-6">
              <ul id="messagesList">
                {listItems}
              </ul>
          </div>
      </div>
    </section>
  );
}