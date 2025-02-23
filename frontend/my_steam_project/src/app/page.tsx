import Image from "next/image";
import GameList from "./Games/GameList";

export default function Home() {
  return (
   <>
      <div>
        <h1>
          <GameList></GameList>
        </h1>
      </div>
   </>
  );
}
