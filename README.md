# Group Knowledge Builder

The Group Knowledge Builder allows a community of people to build up a question-and-answer style knowledge base on a topic they share an interest in.  Examples of potential use cases include:

- A consortium of companies shares a common interest in building up a knowledge base relevant to some aspect of their industry.  They want to share the best answer to important questions they all commonly encounter, but they want to do so in a way that ensures people contributing to the knowledge base are fairly rewarded for their relative contributions and that no one company or person can unduly influence the outcome.

- A company wants to build up a knowledge base on the use of their product to address frequent questions their customers face.  They rely on support forums handled by enthusiast customers, employees contributing as an additional duty, and product evangelist supported through some sort of reward or recognition program.  However, these contributors are concerned that they don’t get fairly rewarded based on the quality of their contributions and customers worry that the knowledge base may be unduly influenced by company pressures on contributors.

- Enthusiast with an interest in a subject area or topic want to build up a high-quality body of knowledge on that subject area or topic.  But potential contributions are fragmented and disorganized and there is little motivation for the widespread body of enthusiast to work together in an organized way to build up that knowledge base.  Potential contributors are concerned their voice won’t be fairly heard and people with great expertise in the field fear their contributions will not be fairly rewarded relative to others.

The Group Knowledge Builder provides a way for a group to build up a body of knowledge in a way that fairly rewards the relative contributions and level of participation of group members.  It helps to ensure that the knowledge base created reflects a genuine community consensus free from biases introduced by external influences.  The resulting knowledge base is also stored in a way that ensures it remains available to all and is not corrupted or distorted in the future.

Potential participants may fall into three broad categories based on different motivations although the contract itself makes no distinction on types of members:

- Experts want to share their expertise and hope that they are rewarded fairly for doing so when the group agrees they provided the best answer to a question.  Experts anticipate they will ultimately get some level of financial reward for their participation.
- Enthusiasts want to participate and have a voice in determining the results of the group’s efforts.  They don’t anticipate a financial reward, but they also don’t intend for their participation to impose a substantial financial cost on themselves.
- Patrons have an interest in seeing the knowledge base created and are willing to make a financial contribution to make that happen even if they don’t have the time or expertise to participate frequently in the group’s efforts.

The knowledge base in controlled by a smart contract which captures questions, answers, and selects the best answer based on ranked choice voting using the minimax voting system.  This system ensures that selection of the best answer reflects a broad and fair consensus of the total group.  The actual content of the questions and answers is stored on the Interplanetary File System so that the content of the questions and answers referenced in the contracts have ensured access, are free from tampering or modification, and are permanent as long as someone, somewhere has an interest in them.

New answers can be proposed, and votes can be changed at any time before the final selection of the best answer is made by the contract.  Note, however, that that a group member is only rewarded for the first time they vote on any one question.

The best answer selection occurs when the required level of participation is reached and a there is a single winner in the ranked voting.  For an initial period of time this means everyone must have voted, however after an initial consensus period the participation requirement will begin to gradually decline until it reaches an established minimum allowed participation level.

Although individual member’s vote may include ties and may also reflect indifference (no vote) on an answer.  The final best answer must be for a distinct group consensus result.  In the unlikely event of a consensus tie for best, revoting will be needed to break the tie.  

The person who establishes a group determines the name of the group.  They also set the minimum required contribution for membership which is based on what their initial contribution was.  Other than these two factors being the founder of group brings no special powers or rewards compared to other group member.

The Group Knowledge Contract maintains a balance of TCRS based solely on what group members contribute through joining and what the remove through withdrawal of tokens.
The contract also maintains a balance of knowledge tokens internally that continually increases to reward members of the group for providing the best answer to a question and for participating through voting on the ranking of proposed answers.  As the number of knowledge tokens increases the value of an individual token in TCRS decreases.  The amount of TCRS a member may withdraw is based on the number of the group’s knowledge tokens they have.

When someone joins the group, they receive an initial knowledge token balance equal to the value of tokens in TCRS at that time.  When a member votes on the best answer to a question, new tokens are created and awarded to them.  When a member’s answer is selected as the best answer, new tokens are created and awarded equal to them.
If a group member provides the best answer frequently, they will be able to withdraw more TCRS than they have contributed.  On the other hand, if a group member does not answer questions and infrequently votes, then they will not be able to withdraw as many TCRS as they contributed.

Groups can form organically and grow, or contract based on how well their members work together to each member’s mutual satisfaction.  Some groups simply won’t work out for any of a wide variety of reasons and this system is designed to allow those groups to phase out without great harm to their participants. 

The Group Knowledge Builder runs locally on your machine and requires the following four component to be running to function:

- WASM PWA KnowledgeGroupBuilder which will run in browser at localhost port 5002.
  - Download a zip file with the application code from GitHub using the button above.
 
- Cirrus Core (Private Net) wallet running locally and accessible over port 38223.
  - https://github.com/stratisproject/CirrusCore/releases/tag/1.6.1.0-privatenet 

- Interplanetary File System node running locally and accessible over port 5001.
  - https://ipfs.io/#install (either the Desktop or CLI install can be used)
  - The IPFS configuration for CORS must be enabled 

  `ipfs config --json API.HTTPHeaders.Access-Control-Allow-Origin '["http://localhost:5002"]'`
  `ipfs config --json API.HTTPHeaders.Access-Control-Allow-Methods '["PUT", "GET", "POST"]'`

- CirrusProxy app.  A very small pass through which is used to access the Cirrus Core API due to CORS restrictions

Use the Cirrus Core wallet’s Create Contract button to deploy the byte code in this repository and included as a solution file in the zip.  Alternatively, the contract can be automatically deployed by running the ContractDeploymentTest found in the solution.

If running the application from Visual Studio make sure to select the Multiple startup projects option and select both the GroupKnowledgeClient and the CirrusProxy app to run.  IPFS and Cirrus Core must also have been started and running for the GroupKnowledgeClient to function.

When the Group Knowledge Builder App opens in your browser you will need to enter an account address from the Cirrus Core Privite Net with TCRS.  Make a note from the wallet of the address of the deployed contract and enter it when prompted.  This will allow you to access the group but not transfer funds or make you a member of the group.  You can click the account icon in the upper right to become a member and be able to use all the apps functionality.

A brief demonstration can be seen here: https://youtu.be/a53xk2k6u0c
 
