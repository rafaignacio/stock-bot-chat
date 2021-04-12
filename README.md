# Finance Chatbot

## Solution Overview

![Solution Overview](./docs/images/sln-overview.png)

## Data Flow

### 1. User AuthN & AuthZ

```mermaid
    sequenceDiagram

    participant u as User
    participant c as Chat
    participant sts as STS

    u->>+c: register
    activate c;
    c->>+sts: resource owner
    sts->>sts: validate credentials
    sts->>sts: generate token
    sts-->>-c: return token
    deactivate c
```

### 2. Chat flow

```mermaid
    sequenceDiagram

    participant u as User
    participant c as Chat
    participant h as Hub

    u ->> c: registers
    c ->> h: create user channel
```

## Deploy instructions

The project was structured using containers for the chatbot application and the backing service, on this case RabbitMQ.
Run the following command to get the project up and running:

```shell
docker-compose up -d
```

## TODO List

[ ] Implement OAuth 2.0 flow
[ ] Improve front end
