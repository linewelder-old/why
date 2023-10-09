const today = new Date();
const isToday = (date) =>
    date.getFullYear() === today.getFullYear() &&
    date.getMonth() === today.getMonth() &&
    date.getDate() === today.getDate();

const times = document.getElementsByTagName('time');
for (const time of times) {
    const timestamp = new Date(time.attributes['datetime'].value);

    time.textContent = timestamp.toLocaleTimeString();
    if (!isToday(timestamp)) {
        time.textContent += ", " + timestamp.toLocaleDateString();
    }
}
