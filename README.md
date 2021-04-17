<!-- PROJECT SHIELDS -->
[![LinkedIn][linkedin-shield]][linkedin-url]



<!-- FRONT PAGE-->
<br />
<p align="center">
  <a href="https://www.um.edu.mt/">
    <img src="UoM.png" alt="Logo" width="100" height="100">
  </a>

  <h2 align="center">A Unified Approach to Distributed Application Development for DLT</h2>
  <h3 align="center">Ryan Falzon</h3>
  <h4 align="center">Supervised by Prof. Gordon Pace</h4>
  <h4 align="center">Co-supervised by Dr Joshua Ellul</h4>
  <h5 align="center">Centre for Distributed Ledger Technologies</h5>
  <h5 align="center">University of Malta</h5>
  <h6 align="center">April 2021</h6>

<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgements">Acknowledgements</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

The widespread interest surrounding blockchain systems, has brought forth the introduction of decentralized applications. Such applications are built using Smart Contracts running on a blockchain network. Due to the siloed nature of blockchains and smart contracts, parts of such applications may have to be executed on different blockchains, or outside the blockchain altogether. For instance, due to privacy constraints arising from GDPR, keeping private data on a public blockchain may not be an options, and would have to be kept on a centralized server which communicates with the blockchain in question.This shift in development methodology introduces new challenges for developers to achieve seamless communication and interaction between off-chain and on-chain code of decentralized applications. The current solution is to program the parts separately including additional code to handle communication between the different systems. Hence, this is considered as a source of additional complexity and also a potential source of error.

In this dissertation, we propose a unified programming model to decentralized application development. We explored techniques that have been used to achieve blockchain interoperability, IoT enabled Smart Contracts, as well as the field of macroprogramming for wireless sensor networks. Our approach  takes a macroprogramming approach, thus allowing for such  systems to be programmed as a monolithic system, but with annotations to add information regarding where each part of the system should be deployed and executed. Ultimately, our aim is to create a development environment where developers can easily explore the placement of data and control flow on different target locations.

In order to demonstrate and evaluate the use of our approach we designed a software system use-case which would require shifting certain components between centralized and decentralized environments. The final results were made possible through the experiment carried out during the evaluation phase. This experiment included development of a number of tasks on the use-case using both the traditional method and the framework proposed herein.

### Built With

* [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)



<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

1.  Download .NET Core 3.1 from [here](https://dotnet.microsoft.com/download).
2.  Download Git from [here](https://git-scm.com/downloads).

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/ryanfalzon/DLT-Dissertation
   ```
2. Build the `UnifiedModel.SourceGenerator` project
   ```sh
   cd Dissertation/src/UnifiedModel/UnifiedModel.SourceGenerator
   dotnet build --output "c:/UnifiedModel/SourceGenerator"
   ```
3. Build the `UnifiedModel.Connectors` project
   ```sh
   cd Dissertation/src/UnifiedModel/UnifiedModel.Connectors
   dotnet build --output "c:/UnifiedModel/Connectors"
   ```
4. Run the `UnifiedModel.SourceGenerator.exe` executable
   ```sh
   UnifiedModel.SourceGenerator.exe "UnifiedCode.txt" "OutputDirectory"
   ```
5. Once generated smart contracts are deployed, fill in the `Ethereum.json` file created in the output directory of step 3
   ```JSON
   {
    "PublicKey": "",
    "PrivateKey": "",
    "Contracts": [
      {
        "Name": "",
        "AbiLocation": "",
        "Address": ""
      }
    ]
   }
   ```
6. Copy the contents of the output directory of step 3 to the location where the desktop code will be running



<!-- USAGE EXAMPLES -->
## Framewok Usage

This is an example of how the framework can be used to create annotated code to be compiled down to C# and Solidity code for Desktop and Etheruem enviornments.
```csharp
[XOn(All)]
public class Profile : XModel("SocialNetwork")
{
  ...
}

[XOn(Ethereum)]
public class SocialNetwork
{
  ...
}
  
[XOn("All")]
public class SocialNetwork
{
  public void Register(Profile profile)
  {
    @XOn("Desktop", profile)
    {
      ...
    }

    ~@XOn("Ethereum", profile)
    {
      ...
    }
  }
}
```



<!-- CONTACT -->
## Contact

[Ryan Falzon](https://www.linkedin.com/in/ryan-falzon-291a3516a/) - rfalzonryan@gmail.com



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
I would like to thank my supervisor Prof. Gordon Pace and co-supervisor Dr. Joshua Ellul for their continual support and guidance throughout this research study. Additionally, I would like to express my gratitude to all lecturers of the Centre of Distributed Ledger Technologies for the knowledge they imparted on throughout their lectures.



<!-- MARKDOWN LINKS & IMAGES -->
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/ryan-falzon-291a3516a/
