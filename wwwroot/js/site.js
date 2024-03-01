// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

"use strict"
let state = "play";
let player_btn = document.getElementById("player_btn");
if (player_btn != null) {
    function playPause() {
        if (state === "play") {
            playbtnImg.src = "/image/pause-weis.png";
            state = "pause";
            video_player.play();
            
        }
        else {
            playbtnImg.src = "/image/play-weis.png";
            state = "play"
            video_player.pause();
        }
    }
    player_btn.onclick = () => {
        playPause()
    }
    

    volumeRange.addEventListener("input", () => {
        video_player.volume = volumeRange.value / 100;
        if (video_player.volume === 0) {
            document.getElementById("volum_sound").src = "/image/audio-volume-off.png"
        }
        else if (video_player.volume > 0){
            document.getElementById("volum_sound").src = "/image/audio-volume-weis.png"
        }
    });
    volumeRange.addEventListener("change", () => {
        video_player.volume = volumeRange.value / 100;
        
    });
}

let existingGenre = document.getElementsByClassName("existing_genre");
for (let it of existingGenre) {
        it.onclick = () => {
        edit_genre.style.display = "block";
        add_genre.style.visibility = "hidden";
        del_genre.style.display = "block";
        input_genre.value = it.textContent;
       
        original_name.value = it.textContent;
     }
}
let existing = document.getElementsByClassName("exGenre");
for (let it of existing) {
    it.onclick = () => {
        
        inpt_crt_gnr.value = it.textContent;
        it.style.color = "lime";
        
    }
}

let delClip = document.getElementById("del_clip_a");
if (delClip != null) {
    delClip.onclick = () => {
        delete_clip.style.display = "block";
    }
    let div_btn_del = document.getElementById("div_btn_del");
    if (div_btn_del !== null) {
        div_btn_del.onclick = () => {
            delete_clip.style.display = "none";
        }
    }
}

let notification = document.getElementById("notification");
if (notification !== null) {
    closeNotification.onclick = () => {
        notification.classList.add("activeClass");
    }
}